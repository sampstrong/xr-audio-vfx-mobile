using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EffectManager : MonoBehaviour
{
    public Preset CurrentPreset => _preset;

    [SerializeField] private List<GameObject> _effects;
    [SerializeField] private SpatialAnchorManager _anchorManager;

    private GameObject _selectedEffect;
    private GameObject _placedEffect;
    
    private TouchAdjustmentHelper _currentTouchHelper;
    private AudioShaderController _currentShaderController;

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
        _anchorManager.CurrentObjectSet += InitPlacedEffect;
    }
    
    public void SetCurrentEffect(int index)
    {
        _selectedEffect = _effects[index];
        Debug.Log($"Current Effect Set To: {_selectedEffect.name}");
    }

    public void PlaceEffect()
    {
        _anchorManager.PlaceAnchor(_selectedEffect);
    }

    private void InitPlacedEffect(GameObject currentEffect)
    {
        _placedEffect = currentEffect;
        
        Debug.Log($"Placed Effect: {_placedEffect.name}");
        
        _currentTouchHelper = _placedEffect.GetComponentInChildren<TouchAdjustmentHelper>();
        _currentShaderController = _placedEffect.GetComponentInChildren<AudioShaderController>();
    }

    public void SetPreset(int presetIndex)
    {
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
        _currentShaderController.Sensitivity = value * 100;
    }
    
    public void ToggleAdjustment()
    {
        Assert.IsNotNull(_placedEffect, "There is no effect placed in the scene");
        Assert.IsNotNull(_currentTouchHelper, "There is no TouchAdjustmentHelper attached to the placed effect");
        _currentTouchHelper.ToggleAdjustmentEnabled();
    }

    public void ToggleRecenter()
    {
        Assert.IsNotNull(_placedEffect, "There is no effect placed in the scene");
        Assert.IsNotNull(_currentTouchHelper, "There is no TouchAdjustmentHelper attached to the placed effect");
        _currentTouchHelper.ToggleRecenterEnabled();
    }


}
