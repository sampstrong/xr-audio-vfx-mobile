using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioController : MonoBehaviour
{
    [Header("Frequency Band")]
    [SerializeField] private int _band;

    protected float _audioAmplitudeBuffer;
    protected float _audioBandIntensityBuffer;

    /// <summary>
    /// Pulls data from AudioSpectrumReader to use to control other variables
    /// </summary>
    protected virtual void Update()
    {
        _audioAmplitudeBuffer = AudioSpectrumReader.amplitudeBuffer; // overall amplitude
        _audioBandIntensityBuffer = AudioSpectrumReader.audioBandIntensityBuffer[_band]; // individual band
    }

    /// <summary>
    /// Gives a float value that can be used to control a variable between a max and min.
    /// Takes input from audioBandIntensityBuffer of audioAmplitudeBuffer
    /// </summary>
    /// <param name="input"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    protected virtual float GetControlValue(float input, float min, float max)
    {
        return (input * (max - min)) + min;
    }
}
