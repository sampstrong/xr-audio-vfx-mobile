using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.AR.Networking.ARNetworkingEventArgs;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.HLAPI;
using Niantic.ARDK.Networking.HLAPI.Authority;
using Niantic.ARDK.Networking.HLAPI.Data;
using Niantic.ARDK.Networking.HLAPI.Object;
using Niantic.ARDK.Networking.HLAPI.Routing;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using UnityEngine;
using UnityEngine.UI;

public class TestApp : MonoBehaviour
  {
    [SerializeField]
    private Button joinButton = null;

    [SerializeField]
    private FeaturePreloadManager preloadManager = null;


    /// References to game objects after instantiation
    private GameObject _ball;

    private GameObject _player;
    private GameObject _playingField;

    /// The score
    public Text peerState;

    /// HLAPI Networking objects
    private IHlapiSession _manager;

    private IAuthorityReplicator _auth;
    private MessageStreamReplicator<Vector3> _hitStreamReplicator;

    
    
    // my variables
    private INetworkedField<Color> _sphereColor;
    [SerializeField] private GameObject _sphere;
    private INetworkedField<bool> _rendererEnabled;
    private Renderer _renderer;

    private INetworkedField<string> _peerStateText;



    private IARNetworking _arNetworking;

    private bool _isHost;
    private IPeer _self;

    
    private bool _synced;

    private void Start()
    {
      ARNetworkingFactory.ARNetworkingInitialized += OnAnyARNetworkingSessionInitialized;

      // _sphere.GetComponent<TestNetworkUpdates>().ColorUpdated += UpdateColor;
      // _sphere.GetComponent<TestNetworkUpdates>().VisibilityToggled += UpdateVisibility;
      

      if (preloadManager.AreAllFeaturesDownloaded())
        OnPreloadFinished(true);
      else
        preloadManager.ProgressUpdated += PreloadProgressUpdated;
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
      if (success)
        joinButton.interactable = true;
      else
        Debug.LogError("Failed to download resources needed to run AR Multiplayer");
    }

    // Every frame, detect if you have hit the ball
    // If so, either bounce the ball (if host) or tell host to bounce the ball
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
      _auth = new GreedyAuthorityReplicator("pongHLAPIAuth", group);

      _auth.TryClaimRole(_isHost ? Role.Authority : Role.Observer, () => {}, () => {});

      var authToObserverDescriptor =
        _auth.AuthorityToObserverDescriptor(TransportType.ReliableUnordered);
      
      
      // added code
      _sphereColor = new NetworkedField<Color>("sphereColor", authToObserverDescriptor, group);
      _sphereColor.ValueChanged += OnColorDidChange;

      _rendererEnabled = new NetworkedField<bool>("rendererEnabled", authToObserverDescriptor, group);
      _rendererEnabled.ValueChanged += OnVisibilityDidChange;

    }

    private void UpdateColor(Color col)
    {
      _sphereColor.Value = col;
    }

    private void OnColorDidChange(NetworkedFieldValueChangedArgs<Color> args)
    {
      var color = args.Value;
      if (!color.HasValue) return;
      
      _sphere.GetComponent<Renderer>().sharedMaterial.color = color.Value;
    }

    private void UpdateVisibility(bool newValue)
    {
      _rendererEnabled.Value = newValue;
    }

    private void OnVisibilityDidChange(NetworkedFieldValueChangedArgs<bool> args)
    {
      var value = args.Value;
      if (!value.HasValue) return;

      _sphere.GetComponent<Renderer>().enabled = value.Value;
    }

    private void OnAnyARNetworkingSessionInitialized(AnyARNetworkingInitializedArgs args)
    {
      _arNetworking = args.ARNetworking;
      _arNetworking.PeerStateReceived += OnPeerStateReceived;
      _arNetworking.Networking.Connected += OnDidConnect;
    }

    private void OnDestroy()
    {
      ARNetworkingFactory.ARNetworkingInitialized -= OnAnyARNetworkingSessionInitialized;

      if (_arNetworking != null)
      {
        _arNetworking.PeerStateReceived -= OnPeerStateReceived;
        _arNetworking.Networking.Connected -= OnDidConnect;
      }
    }
  }
