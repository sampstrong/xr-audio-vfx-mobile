using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelShaderController : AudioController
{
    [SerializeField] private Material _pixelMat;

    protected override void Update()
    {
        base.Update();

        var intensity = Mathf.Clamp(_audioBandIntensityBuffer * 100, 0.0f, 1.0f);

        var controlValue = GetControlValue(intensity, 0.1f, 1.0f);

        _pixelMat.SetFloat("_OpacityMultiplier", controlValue);
    }
}
