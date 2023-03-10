using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioVFXController : AudioController
{
    public bool Dissolve { get => _controlDissolve; }
    public bool Rescaled { get => _rescaled; set => _rescaled = value; }
    
    [SerializeField] private List<VisualEffect> _vFX;
    
    [Header("Play Rate")] 
    [SerializeField] private bool _controlPlayRate;
    [SerializeField] private float _minRate, _maxRate;

    [Header("Color")] 
    [SerializeField] private bool _controlColor;
    [SerializeField] private float _colorThreshold = 0.5f;

    [Header("Turbulence Frequency")] 
    [SerializeField] private bool _controlFrequency;
    [SerializeField] private float _minFreq, _maxFreq;

    [Header("Dissolve Amount")] 
    [SerializeField] private bool _controlDissolve;
    [SerializeField] private bool _rescaled = false;
    
    
    protected override void Update()
    {
        base.Update();

        if (_controlPlayRate)  ControlPlayRate(_audioBandIntensityBuffer);
        if (_controlColor)     ControlColor(_audioBandIntensityBuffer);
        if (_controlFrequency) ControlFrequency(_audioBandIntensityBuffer);
        if (_controlDissolve)  ControlDissolve(_audioBandIntensityBuffer);
    }

    private void ControlPlayRate(float intensity)
    {
        foreach (var effect in _vFX)
        {
            effect.playRate = GetControlValue(intensity, _minRate, _maxRate);
        }
    }

    private void ControlColor(float intensity)
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

    private void ControlFrequency(float intensity)
    {
        foreach (var effect in _vFX)
        {
            effect.SetFloat("TurbulenceFreq", GetControlValue(intensity, _minFreq, _maxFreq));
        }
    }

    private void ControlDissolve(float intensity)
    {
        // use if you want values to be flipped
        // var amount = -intensity + 1;
        
        float sensitivity;
        if (_rescaled) sensitivity = 0.7f;
        else sensitivity = 1;
        
        var amount = intensity * sensitivity;
        
        foreach (var effect in _vFX)
        {
            effect.SetFloat("DissolveAmount", amount);
        }
    }
}
