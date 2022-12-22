using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioVFXController : AudioController
{
    [SerializeField] private float _minRate, _maxRate;
    [SerializeField] private float _colorThreshold = 0.5f;
    
    [SerializeField] private List<VisualEffect> _vFX;
    

    protected override void Update()
    {
        base.Update();
        
        ControlVFXSpeed(_audioBandIntensityBuffer);
        ControlVFXColor(_audioBandIntensityBuffer);
    }

    private void ControlVFXSpeed(float intensity)
    {
        foreach (var effect in _vFX)
        {
            //effect.playRate = (intensity * _maxRate) + _minRate;
            effect.playRate = GetControlValue(intensity, _minRate, _maxRate);
        }
    }

    private void ControlVFXColor(float intensity)
    {
        if (intensity > _colorThreshold) SetColor(true);
        else SetColor(false);
    }

    private void SetColor(bool toggle)
    {
        foreach (var effect in _vFX)
        {
            effect.SetBool("ColorToggle", toggle);
        }
    }
}
