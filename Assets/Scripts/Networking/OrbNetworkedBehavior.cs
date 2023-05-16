using System;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.HLAPI.Authority;
using Niantic.ARDK.Networking.HLAPI.Data;
using Niantic.ARDK.Networking.HLAPI.Object;
using Niantic.ARDK.Networking.HLAPI.Object.Unity;
using UnityEngine;

[RequireComponent(typeof(AuthBehaviour))]
public class OrbNetworkedBehavior : NetworkedBehaviour
{
    [SerializeField] private Orb _orb;

    private NetworkedField<Vector3> _networkedOrigin;
    private NetworkedField<int> _networkedBand;
    private NetworkedField<bool> _networkedEnabled;

    public event Action<Vector3> OriginChangeReceived;
    public event Action<int> BandChangeReceived;
    public event Action<bool> EnabledChangeReceived;

    protected override void SetupSession
    (
        out Action initializer,
        out int order
    )
    {
        initializer = () =>
        {
            NetworkedDataDescriptor unreliableDescriptor =
                Owner.Auth.AuthorityToObserverDescriptor(TransportType.UnreliableUnordered);
            
            NetworkedDataDescriptor reliableDescriptor =
                Owner.Auth.AuthorityToObserverDescriptor(TransportType.ReliableUnordered);
            

            new UnreliableBroadcastTransformPacker
            (
                "netTransform",
                transform,
                reliableDescriptor,
                TransformPiece.All,
                Owner.Group
            );
            
            _networkedOrigin = new NetworkedField<Vector3>
            (
                "origin",
                reliableDescriptor,
                Owner.Group
            );
            _networkedOrigin.ValueChanged += OnOriginChanged;

            _networkedBand = new NetworkedField<int>
            (
                "id",
                reliableDescriptor,
                Owner.Group
            );
            _networkedBand.ValueChanged += OnBandChanged;

            _networkedEnabled = new NetworkedField<bool>
            (
                "enabled",
                reliableDescriptor,
                Owner.Group
            );
            _networkedEnabled.ValueChanged += OnEnabledChanged;

            _orb.OriginChangeSent += UpdateOriginForAllPeers;
            _orb.BandChangeSent += UpdateIdForAllPeers;
            _orb.EnabledChangeSent += UpdateEnabledForAllPeers;
        };

        order = 0;
    }

    private void UpdateOriginForAllPeers(Vector3 origin)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedOrigin.Value = origin;
    }

    private void UpdateIdForAllPeers(int id)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedBand.Value = id;
    }

    private void UpdateEnabledForAllPeers(bool enabled)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedEnabled.Value = enabled;
    }

    private void OnOriginChanged(NetworkedFieldValueChangedArgs<Vector3> args)
    {
        var value = args.Value;
        if (!value.HasValue) return;

        var origin = value.Value;
        
        OriginChangeReceived?.Invoke(origin);
    }

    private void OnBandChanged(NetworkedFieldValueChangedArgs<int> args)
    {
        var value = args.Value;
        if (!value.HasValue) return;

        var id = value.Value;

        BandChangeReceived?.Invoke(id);
    }

    private void OnEnabledChanged(NetworkedFieldValueChangedArgs<bool> args)
    {
        var value = args.Value;
        if (!value.HasValue) return;

        var enabled = value.Value;

        EnabledChangeReceived?.Invoke(enabled);
    }

    private void OnDestroy()
    {
        if (_networkedBand != null)
            _networkedBand.ValueChanged -= OnBandChanged;
        
        if (_networkedBand != null)
            _networkedEnabled.ValueChanged -= OnEnabledChanged;
        
        _orb.BandChangeSent -= UpdateIdForAllPeers;
        _orb.EnabledChangeSent -= UpdateEnabledForAllPeers;
    }
}
