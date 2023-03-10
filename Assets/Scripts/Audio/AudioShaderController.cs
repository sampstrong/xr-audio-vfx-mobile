using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioShaderController : AudioController
{
    public float Sensitivity
    {
        get => _sensitivity;
        set => SetSensitivity(value);
    }
    
    [SerializeField] private Material _pixelMat;
    [SerializeField] private string _shaderParameter = "_OpacityMultiplier";
    
    [SerializeField] private float _sensitivity = 100.0f;

    
    protected override void Update()
    {
        base.Update();

        var intensity = Mathf.Clamp(_audioBandIntensityBuffer * _sensitivity, 0.0f, 1.0f);

        var controlValue = GetControlValue(intensity, 0.1f, 1.0f);

        _pixelMat.SetFloat(_shaderParameter, controlValue);
    }

    private void SetSensitivity(float value)
    {
        _sensitivity = value;
        Debug.Log($"Sensitivity: {_sensitivity}");
    }
    
}
