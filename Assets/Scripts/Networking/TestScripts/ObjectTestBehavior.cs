using System;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.HLAPI.Authority;
using Niantic.ARDK.Networking.HLAPI.Data;
using Niantic.ARDK.Networking.HLAPI.Object;
using Niantic.ARDK.Networking.HLAPI.Object.Unity;
using UnityEngine;

[RequireComponent(typeof(AuthBehaviour))]
public class ObjectTestBehavior : NetworkedBehaviour
{
    [SerializeField] private ObjectTest _object;

    private NetworkedField<Color> _networkedColorField;
    private NetworkedField<int> _networkedId;
    private NetworkedField<bool> _networkedEnabled;

    public event Action<Color> ColorChangeReceived;
    public event Action<int> IdChangeReceived;
    public event Action<bool> EnabledChangeReceived;

    protected override void SetupSession
    (
        out Action initializer,
        out int order
    )
    {
        initializer = () =>
        {
            NetworkedDataDescriptor authToObserverDescriptor =
                Owner.Auth.AuthorityToObserverDescriptor(TransportType.UnreliableUnordered);

            new UnreliableBroadcastTransformPacker
            (
                "netTransform",
                transform,
                authToObserverDescriptor,
                TransformPiece.All,
                Owner.Group
            );

            _networkedColorField = new NetworkedField<Color>
            (
                "color",
                authToObserverDescriptor,
                Owner.Group
            );
            _networkedColorField.ValueChanged += OnColorChanged;

            _networkedId = new NetworkedField<int>
            (
                "id",
                authToObserverDescriptor,
                Owner.Group
            );
            _networkedId.ValueChanged += OnIdChanged;

            _networkedEnabled = new NetworkedField<bool>
            (
                "enabled",
                authToObserverDescriptor,
                Owner.Group
            );
            _networkedEnabled.ValueChanged += OnEnabledChanged;

            _object.ColorChangeSent += UpdateColorForAllPeers;
            _object.IdChangeSent += UpdateIdForAllPeers;
            _object.EnabledChangeSent += UpdateEnabledForAllPeers;
        };

        order = 0;
    }

    
    private void UpdateColorForAllPeers(Color newColor)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedColorField.Value = newColor;
    }

    public void UpdateIdForAllPeers(int id)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedId.Value = id;
    }

    public void UpdateEnabledForAllPeers(bool enabled)
    {
        if (Owner.Auth.LocalRole != Role.Authority) return;
        _networkedEnabled.Value = enabled;
    }

    private void OnColorChanged(NetworkedFieldValueChangedArgs<Color> args)
    {
        var value = args.Value;
        if (!value.HasValue) return;

        var color = value.Value;

        ColorChangeReceived?.Invoke(color);
    }

    private void OnIdChanged(NetworkedFieldValueChangedArgs<int> args)
    {
        var value = args.Value;
        if (!value.HasValue) return;

        var id = value.Value;

        IdChangeReceived?.Invoke(id);
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
        _networkedColorField.ValueChanged -= OnColorChanged;
        _networkedId.ValueChanged -= OnIdChanged;
        _networkedEnabled.ValueChanged -= OnEnabledChanged;
        
        _object.ColorChangeSent -= UpdateColorForAllPeers;
        _object.IdChangeSent -= UpdateIdForAllPeers;
        _object.EnabledChangeSent -= UpdateEnabledForAllPeers;
    }
}
