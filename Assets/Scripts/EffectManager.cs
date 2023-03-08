using System.Collections;
using System.Collections.Generic;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;
using UnityEngine.VFX;

public class EffectManager : Singleton<EffectManager>
{
    public Preset CurrentPreset => _preset;
    
    [SerializeField] private List<GameObject> _effects;
    [SerializeField] private SpatialAnchorManager _anchorManager;

    private GameObject _currentEffect;
    
    public enum Preset
    {
        Base = 0,
        Build = 1,
        Drop = 2,
        Break = 3,
    }

    private Preset _preset;

    private void Start()
    {
        _preset = Preset.Base;
    }
    
    public void SetCurrentEffect(int index)
    {
        _currentEffect = _effects[index];
        Debug.Log($"Current Effect Set To: {_effects[index].name}");
    }

    public void PlaceEffect()
    {
        _anchorManager.PlaceAnchor(_currentEffect);
    }
    
    private void Update()
    {
        if (PlatformAgnosticInput.touchCount > 0)
        {
            _preset = Preset.Drop;
        }
        else
        {
            _preset = Preset.Base;
        }
    }
}
