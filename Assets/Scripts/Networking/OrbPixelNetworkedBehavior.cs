using System;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.HLAPI.Authority;
using Niantic.ARDK.Networking.HLAPI.Data;
using Niantic.ARDK.Networking.HLAPI.Object;
using Niantic.ARDK.Networking.HLAPI.Object.Unity;
using UnityEngine;

[RequireComponent(typeof(AuthBehaviour))]
public class OrbPixelNetworkedBehavior : NetworkedBehaviour
{
    [SerializeField] private OrbPixel _orbPixel;
    
    private NetworkedField<bool> _networkedEnabled;

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

            _networkedEnabled = new NetworkedField<bool>
            (
                "enabled",
                reliableDescriptor,
                Owner.Group
            );
            _networkedEnabled.ValueChangedIfReceiver += OnEnabledChanged;

            _orbPixel.EnabledChangeSent += UpdateEnabledForAllPeers;
        };

        order = 0;
    }

    private void UpdateEnabledForAllPeers(bool enabled)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedEnabled.Value = enabled;
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
        if (_networkedEnabled != null)
            _networkedEnabled.ValueChangedIfReceiver -= OnEnabledChanged;

        _orbPixel.EnabledChangeSent -= UpdateEnabledForAllPeers;
    }
}
