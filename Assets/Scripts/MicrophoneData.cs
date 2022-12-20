using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneData : MonoBehaviour
{
    private AudioClip _microphoneClip;
    private int _sampleWindow = 64;
    
    // Start is called before the first frame update
    void Start()
    {
        MicroPhoneToAudioClip();
    }

    private void MicroPhoneToAudioClip()
    {
        string microphoneName = Microphone.devices[0];
        _microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
        Debug.Log($"Mic: {microphoneName}");
    }

    public float GetLoudnessFromMicrophone()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), _microphoneClip);
    }

    private float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosisiton = clipPosition - _sampleWindow;

        if (startPosisiton < 0) return 0;
        
        float[] waveData = new float[_sampleWindow];
        var clipDataReceived = clip.GetData(waveData, startPosisiton);
        
        Debug.Log($"Clip Data Received: {clipDataReceived}");

        // compute the mean value of the loudness by adding the intensity of
        // each sample together and dividing by the total number of samples
        float totalLoudness = 0f;

        for (int i = 0; i < _sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / _sampleWindow;
    }
}
