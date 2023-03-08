using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGeometryController : AudioController
{
    [SerializeField] private float _minScale = 0.3f;
    [SerializeField] private float _maxScale = 1f;
    
    protected override void Update()
    {
        base.Update();
        ControlScale();
    }

    private void ControlScale()
    {
        transform.localScale = Vector3.one * GetControlValue(_audioBandIntensityBuffer, _minScale, _maxScale);
    }
    
}
