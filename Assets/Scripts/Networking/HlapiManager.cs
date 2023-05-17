using System;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.AR.Networking.ARNetworkingEventArgs;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.HLAPI;
using Niantic.ARDK.Networking.HLAPI.Authority;
using Niantic.ARDK.Networking.HLAPI.Routing;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HlapiManager : MonoBehaviour
{
    public static bool IsHost => _isHost;
    
    [Header("UI")]
    [SerializeField] private Button joinButton = null;
    [SerializeField] private FeaturePreloadManager preloadManager = null;
    [SerializeField] private ARNetworkingManager _networkingManager;
    [SerializeField] private TextMeshProUGUI peerState;

    /// Networking objects
    private IHlapiSession _manager;
    private IAuthorityReplicator _auth;
    private IARNetworking _arNetworking;
    private IPeer _self;
    private static bool _isHost;
    private bool _synced;

    private void Start()
    {
      ARNetworkingFactory.ARNetworkingInitialized += OnAnyARNetworkingSessionInitialized;

      if (preloadManager.AreAllFeaturesDownloaded())
        OnPreloadFinished(true);
      else
        preloadManager.ProgressUpdated += PreloadProgressUpdated;
      
      
    }

    private void OnAnchorsAdded(AnchorsArgs args)
    {
      Debug.Log($"Anchor Count: {args.Anchors.Count}");
    }

    private void PreloadProgressUpdated(FeaturePreloadManager.PreloadProgressUpdatedArgs args)
    {
      if (args.PreloadAttemptFinished)
      {
        preloadManager.ProgressUpdated -= PreloadProgressUpdated;
        OnPreloadFinished(args.FailedPreloads.Count == 0);
      }
    }

    private void OnPreloadFinished(bool success)
    {
      if (!success)
        Debug.LogError("Failed to download resources needed to run AR Multiplayer");
    }

   
    private void Update()
    {
      if (_manager != null)
        _manager.SendQueuedData();
    }

    private void OnPeerStateReceived(PeerStateReceivedArgs args)
    {
      if (_self.Identifier != args.Peer.Identifier)
      {
        if (args.State == PeerState.Stable)
          _synced = true;
      }

      string message = args.State.ToString();
      peerState.text = message;
      Debug.Log("We reached state " + message);
    }

    private void OnDidConnect(ConnectedArgs connectedArgs)
    {
      _isHost = connectedArgs.IsHost;
      _self = connectedArgs.Self;

      _manager = new HlapiSession(19244);

      var group = _manager.CreateAndRegisterGroup(new NetworkId(4321));
      _auth = new GreedyAuthorityReplicator("audioVfxAuth", group);

      _auth.TryClaimRole(_isHost ? Role.Authority : Role.Observer, () => {}, () => {});

      // var authToObserverDescriptor =
      //   _auth.AuthorityToObserverDescriptor(TransportType.ReliableUnordered);

    }

    private void OnAnyARNetworkingSessionInitialized(AnyARNetworkingInitializedArgs args)
    {
      _arNetworking = args.ARNetworking;
      _arNetworking.PeerStateReceived += OnPeerStateReceived;
      _arNetworking.Networking.Connected += OnDidConnect;
      _arNetworking.ARSession.AnchorsAdded += OnAnchorsAdded;
    }

    private void OnApplicationQuit()
    {
      CleanUp();
    }

    private void OnDestroy()
    {
      ARNetworkingFactory.ARNetworkingInitialized -= OnAnyARNetworkingSessionInitialized;
      CleanUp();
    }

    private void CleanUp()
    {
      if (_arNetworking != null)
      {
        _arNetworking.PeerStateReceived -= OnPeerStateReceived;
        _arNetworking.Networking.Connected -= OnDidConnect;
        _arNetworking.ARSession.Dispose();
        _arNetworking.Networking.Dispose();
        _arNetworking.Dispose();
        
      }
      if (_manager != null)
      {
        _manager.Networking.Leave();
        _manager.Networking.Dispose();
        _manager.Dispose();
      }
      if (_networkingManager != null)
      {
        _networkingManager.NetworkSessionManager.Deinitialize();
      }
    }
  }


