using System;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectTest : MonoBehaviour
{
    [SerializeField] private ObjectTestBehavior _objectBehavior;
    [SerializeField] private Renderer _renderer;

    private int _id;

    public event Action<Color> ColorChangeSent;
    public event Action<int> IdChangeSent;
    public event Action<bool> EnabledChangeSent;

    private void Start()
    {
        _objectBehavior.ColorChangeReceived += ReceiveColorUpdates;
        _objectBehavior.IdChangeReceived += ReceiveIdUpdates;
        _objectBehavior.EnabledChangeReceived += ReceiveEnabledUpdates;
    }

    private void Update()
    {
        if (PlatformAgnosticInput.touchCount <= 0) return;
        var touch = PlatformAgnosticInput.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            _renderer.sharedMaterial.color = Random.ColorHSV();
            ColorChangeSent?.Invoke(_renderer.sharedMaterial.color);
            
            var id = Random.Range(1, 100);
            IdChangeSent?.Invoke(id);
            
            _renderer.enabled = true;
            EnabledChangeSent?.Invoke(_renderer.enabled);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            _renderer.enabled = false;
            EnabledChangeSent?.Invoke(_renderer.enabled);
        }
    }
    
    
    private void ReceiveColorUpdates(Color color)
    {
        _renderer.sharedMaterial.color = color;
    }

    private void ReceiveIdUpdates(int id)
    {
        _id = id;
        // Debug.Log($"New ID: {id}");
    }

    private void ReceiveEnabledUpdates(bool enabled)
    {
        _renderer.enabled = enabled;
    }

    private void OnDestroy()
    {
        _objectBehavior.ColorChangeReceived -= ReceiveColorUpdates;
        _objectBehavior.IdChangeReceived -= ReceiveIdUpdates;
        _objectBehavior.EnabledChangeReceived -= ReceiveEnabledUpdates;
    }
}
