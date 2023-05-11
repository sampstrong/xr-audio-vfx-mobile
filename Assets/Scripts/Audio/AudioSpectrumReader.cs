using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[RequireComponent(typeof(AudioSource))]
public class AudioSpectrumReader : MonoBehaviour
{
    private AudioSource _audioSource;
    
    // microphone input
    public AudioClip audioClip;
    public bool useMicrophone;
    private string selectedDevice;
    public AudioMixerGroup mixerGroupMicrophone, mixerGroupMaster;

    private float[] _samplesLeft = new float[512];
    private float[] _samplesRight = new float[512];

    public static float[] freqBand = new float[8];
    public static float[] bandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];

    public static float[] _freqBandHighest = new float[8];
    public static float[] audioBandIntensity = new float[8];
    public static float[] audioBandIntensityBuffer = new float[8];

    public static float amplitude, amplitudeBuffer;
    private float amlitudeHighest;

    public float audioProfile = 5;

    public enum Channel {Stereo, Left, Right};
    public Channel channel = new Channel();

    
    private IEnumerator Start()
    {
        _audioSource = GetComponent<AudioSource>();
        SetAudioProfile(audioProfile);

        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("Microphone permission granted");
        }
        else
        {
            Debug.Log("Microphone permission not granted");
        }

        
        // microphone input
        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                selectedDevice = Microphone.devices[0];
                _audioSource.outputAudioMixerGroup = mixerGroupMicrophone;
                _audioSource.clip = Microphone.Start(selectedDevice, true, 10, AudioSettings.outputSampleRate);
                if (Microphone.IsRecording(selectedDevice))
                {
                    while (!(Microphone.GetPosition(selectedDevice) > 0))
                    {
                        // wait until the microphone has started
                    }
                    _audioSource.Play();
                    _audioSource.loop = true;
                    Debug.Log($"Recording started with {selectedDevice}: {Microphone.IsRecording(selectedDevice)}");
                }
            }
            else
            {
                useMicrophone = false;
            }
        }
        else
        {
            _audioSource.outputAudioMixerGroup = mixerGroupMaster;
            _audioSource.clip = audioClip;
        }
    }
    
    void Update()
    {
        
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        MakeBandBuffer();
        CreateAudioBandIntensity();
        GetAmplitude();

    }

    void SetAudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    void GetAmplitude()
    {

        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;

        for (int i = 0; i < 8; i++)
        {
            currentAmplitude += audioBandIntensity[i];
            currentAmplitudeBuffer += audioBandIntensityBuffer[i];
        }

        if(currentAmplitude > amlitudeHighest)
        {
            amlitudeHighest = currentAmplitude;
        }

        amplitude = currentAmplitude / amlitudeHighest;
        amplitudeBuffer = currentAmplitudeBuffer / amlitudeHighest;
    }


    void CreateAudioBandIntensity()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = freqBand[i];
            }

            audioBandIntensity[i] = (freqBand[i] / _freqBandHighest[i]);
            audioBandIntensityBuffer[i] = (bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void MakeBandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (freqBand[g] > bandBuffer[g])
            {
                bandBuffer[g] = freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }
            if (freqBand[g] < bandBuffer[g])
            {
                bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        _audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
    }

    

    void MakeFrequencyBands()
    {
        /* 22050 Hz / 512 samples = 43Hz per sample
         * 
         * band 0: 2 samples = 86Hz: 0 - 86
         * band 1: 4 samples = 172Hz: 87 - 258
         * band 2: 8 samples = 344Hz: 259 - 602
         * band 3: 16 samples = 688Hz: 603 - 1290
         * band 4: 32 samples = 1376Hz: 1291 - 2666
         * band 5: 64 samples = 2752Hz: 2667 - 5418
         * band 6: 128 samples = 5504Hz: 5419 - 10922
         * band 7: 256 samples = 11008Hz: 10923 - 21930
         *
         *Total = 510 -- 2 short of 512 -- can add 2 to band 7 below
         */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            /*
            if (i == 7)
            {
                sampleCount += 2;
            }
            */

            for (int j = 0; j < sampleCount; j++)
            {

                if(channel == Channel.Stereo)
                {
                    average += (_samplesLeft[count] + _samplesRight[count]) * (count + 1);
                }
                if(channel == Channel.Left)
                {
                    average += _samplesLeft[count] * (count + 1);
                }
                if (channel == Channel.Right)
                {
                    average += _samplesRight[count] * (count + 1);
                }

                count++;
            }

            average /= count;

            freqBand[i] = average * 10;

        }


    }
}
