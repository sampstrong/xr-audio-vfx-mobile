using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class VerticalShaft : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    
    private VerticalShaftGroup _shaftGroup;
    private PixelGlitch _pixelGlitch;
    private int _index;

    private enum StrobeState
    {
        Off = 0,
        Synchronized = 1,
        Randomized = 2
    }

    private StrobeState _strobeState;

    private float _randomOffset;
    private float _strobeSpeed = 10.0f;
    
    public void Init(int index,VerticalShaftGroup group, PixelGlitch pixelGlitch)
    {
        _index = index;
        _shaftGroup = group;
        _pixelGlitch = pixelGlitch;
        
        _pixelGlitch.PositionUpdated += UpdateNodePosition;

        _randomOffset = Random.Range(0, 100);
    }

    private void Update()
    {
        if (PlatformAgnosticInput.touchCount <= 0) return;
        
        var touch = PlatformAgnosticInput.GetTouch(0);
        Strobe();
    }

    private void UpdateNodePosition(int index, Vector3 pos)
    {
        if (index != _index) return;
        transform.position = pos;
    }

    private void Strobe()
    {
        var triggerValue = Mathf.Sin((Time.time + _randomOffset) * _strobeSpeed);

        if (triggerValue >= -0.5)
            _renderer.enabled = false;
        else if (triggerValue <= 0.5)
            _renderer.enabled = true;
    }
}