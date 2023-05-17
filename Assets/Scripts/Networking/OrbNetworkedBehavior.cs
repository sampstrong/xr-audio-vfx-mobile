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

    
    private NetworkedField<Vector3> _networkedVelocity;
    private NetworkedField<Vector3> _networkedOrigin;
    private NetworkedField<int> _networkedBand;
    private NetworkedField<float> _networkedBandIntensity;
    private NetworkedField<bool> _networkedEnabled;

   
    public event Action<Vector3> VelocityChangeReceived; 
    public event Action<Vector3> OriginChangeReceived;
    public event Action<float> BandIntensityChangeReceived;
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
                Owner.Auth.AuthorityToObserverDescriptor(TransportType.ReliableOrdered);
            
            // movement seems to be choppy on receiving end
            new UnreliableBroadcastTransformPacker
            (
                "netTransform",
                transform,
                reliableDescriptor,
                TransformPiece.All,
                Owner.Group
            );
            
            _networkedVelocity = new NetworkedField<Vector3>
            (
                "velocity",
                reliableDescriptor,
                Owner.Group
            );
            _networkedVelocity.ValueChangedIfReceiver += OnVelocityChanged;
            
            _networkedOrigin = new NetworkedField<Vector3>
            (
                "origin",
                reliableDescriptor,
                Owner.Group
            );
            _networkedOrigin.ValueChangedIfReceiver += OnOriginChanged;

            _networkedBand = new NetworkedField<int>
            (
                "band",
                reliableDescriptor,
                Owner.Group
            );
            _networkedBand.ValueChangedIfReceiver += OnBandChanged;
            
            _networkedBandIntensity = new NetworkedField<float>
            (
                "bandIntensity",
                reliableDescriptor,
                Owner.Group
            );
            _networkedBandIntensity.ValueChangedIfReceiver += OnBandIntensityChanged;

            _networkedEnabled = new NetworkedField<bool>
            (
                "enabled",
                reliableDescriptor,
                Owner.Group
            );
            _networkedEnabled.ValueChangedIfReceiver += OnEnabledChanged;

            
            _orb.VelocityChangeSent += UpdateVelocityForAllPeers;
            _orb.OriginChangeSent += UpdateOriginForAllPeers;
            _orb.BandChangeSent += UpdateBandForAllPeers;
            _orb.BandIntensityChangeSent += UpdateBandIntensityForAllPeers;
            _orb.EnabledChangeSent += UpdateEnabledForAllPeers;
        };

        order = 0;
    }

    private void UpdateVelocityForAllPeers(Vector3 velocity)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedVelocity.Value = velocity;
    }

    private void UpdateOriginForAllPeers(Vector3 origin)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedOrigin.Value = origin;
    }

    private void UpdateBandForAllPeers(int id)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedBand.Value = id;
    }

    private void UpdateBandIntensityForAllPeers(float bandIntensity)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedBandIntensity.Value = bandIntensity;
    }

    private void UpdateEnabledForAllPeers(bool enabled)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedEnabled.Value = enabled;
    }

    private void OnVelocityChanged(NetworkedFieldValueChangedArgs<Vector3> args)
    {
        var value = args.Value;
        if (!value.HasValue) return;

        var velocity = value.Value;
        
        VelocityChangeReceived?.Invoke(velocity);
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
    
    private void OnBandIntensityChanged(NetworkedFieldValueChangedArgs<float> args)
    {
        var value = args.Value;
        if (!value.HasValue) return;

        var bandIntensity = value.Value;

        BandIntensityChangeReceived?.Invoke(bandIntensity);
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
        if (_networkedVelocity != null)
            _networkedVelocity.ValueChangedIfReceiver -= OnVelocityChanged;
        if (_networkedOrigin != null)
            _networkedOrigin.ValueChangedIfReceiver -= OnOriginChanged;
        if (_networkedBand != null)
            _networkedBand.ValueChangedIfReceiver -= OnBandChanged;
        if(_networkedBandIntensity != null)
            _networkedBandIntensity.ValueChangedIfReceiver -= OnBandIntensityChanged;
        if (_networkedEnabled != null)
            _networkedEnabled.ValueChangedIfReceiver -= OnEnabledChanged;

        _orb.VelocityChangeSent -= UpdateVelocityForAllPeers;
        _orb.OriginChangeSent -= UpdateOriginForAllPeers;
        _orb.BandChangeSent -= UpdateBandForAllPeers;
        _orb.BandIntensityChangeSent -= UpdateBandIntensityForAllPeers;
        _orb.EnabledChangeSent -= UpdateEnabledForAllPeers;
    }
}
