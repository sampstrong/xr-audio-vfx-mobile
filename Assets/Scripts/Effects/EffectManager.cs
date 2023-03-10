using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

public class EffectManager : Singleton<EffectManager>
{
    public Preset CurrentPreset => _preset;

    [SerializeField] private List<GameObject> _effects;
    [SerializeField] private SpatialAnchorManager _anchorManager;

    private GameObject _currentEffect;
    private TouchAdjustmentHelper _currentTouchHelper;

    private Vector2 _touchStartPos;
    private Vector2 _touchEndPos;
    
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
        _currentTouchHelper = _currentEffect.GetComponentInChildren<TouchAdjustmentHelper>();
        Debug.Log($"Current Effect Set To: {_effects[index].name}");
    }

    public void PlaceEffect()
    {
        _anchorManager.PlaceAnchor(_currentEffect);
    }

    public void SetPreset(int presetIndex)
    {
        if (_currentEffect == null) return;
        
        _preset = (Preset)presetIndex;
        
        switch (_preset)
        {
            case Preset.Base:
                VFXEventManager.InvokeBaseStartedEvent();
                break;
            case Preset.Build:
                VFXEventManager.InvokeBuildStartedEvent();
                break;
            case Preset.Drop:
                VFXEventManager.InvokeDropStartedEvent();
                break;
            case Preset.Break:
                VFXEventManager.InvokeBreakStartedEvent();
                break;
        }
    }

    public void AdjustSensitivity(float value)
    {
        
    }
    
    public void ToggleAdjustment()
    {
        Assert.IsNotNull(_currentTouchHelper, "There is no TouchAdjustmentHelper attached to the current effect");
        _currentTouchHelper.ToggleAdjustmentEnabled();
    }

    public void ToggleRecenter()
    {
        Assert.IsNotNull(_currentTouchHelper, "There is no TouchAdjustmentHelper attached to the current effect");
        _currentTouchHelper.ToggleRecenterEnabled();
    }


}
