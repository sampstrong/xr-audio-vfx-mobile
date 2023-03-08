using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioShaderController : AudioController
{
    [SerializeField] private Material _pixelMat;

    [Range(0.0f, 100.0f)]
    [SerializeField] private float _sensitivity = 100.0f;

    protected override void Update()
    {
        base.Update();

        var intensity = Mathf.Clamp(_audioBandIntensityBuffer * _sensitivity, 0.0f, 1.0f);

        var controlValue = GetControlValue(intensity, 0.1f, 1.0f);

        _pixelMat.SetFloat("_OpacityMultiplier", controlValue);
    }
}
