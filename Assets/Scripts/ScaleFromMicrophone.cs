using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFromMicrophone : MonoBehaviour
{
    [SerializeField] private Vector3 _minScale;
    [SerializeField] private Vector3 _maxScale;
    private MicrophoneData _micData;

    [SerializeField] private float _sensitivity = 100;
    [SerializeField] private float threshold = 0.1f;

    private void Start()
    {
        _micData = FindObjectOfType<MicrophoneData>();
    }
    
    void Update()
    {
        var loudness = _micData.GetLoudnessFromMicrophone() * _sensitivity;

        if (loudness < threshold) loudness = 0;
        transform.localScale = Vector3.Lerp(_minScale, _maxScale, loudness);

        if (loudness <= 0) return;
        Debug.Log($"Loudness: {loudness}");
    }
}
