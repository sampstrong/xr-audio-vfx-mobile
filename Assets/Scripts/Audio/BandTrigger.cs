using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Creates global trigger events for each frequency band when they get above a certain intensity
/// </summary>
public class BandTrigger : MonoBehaviour
{
    public Band[] Bands { get => _bands; }

    [SerializeField] private float _sensitivity = 0.5f;

    private float _triggerThreshold = 0.5f;
    
    private Band[] _bands = new Band[8];

    /// <summary>
    /// Creates an array of bands and initializes each one
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < _bands.Length; i++)
        {
            _bands[i] = new Band(0, false);
        }
        
        SetSensitivity(_sensitivity);
    }
    
    /// <summary>
    /// Pulls data for each band from the AudioSpectrumReader and feeds it into the trigger
    /// </summary>
    private void Update()
    {
        for (int i = 0; i < _bands.Length; i++)
        {
            _bands[i].Intensity = AudioSpectrumReader.audioBandIntensityBuffer[i];
        }
        
        GetBandTrigger();
    }

    /// <summary>
    /// Controls events for each band for triggered and released
    /// </summary>
    private void GetBandTrigger()
    {
        for (int i = 0; i < _bands.Length; i++)
        {
            if (_bands[i].Intensity > _triggerThreshold && !_bands[i].Triggered)
            {
                _bands[i].Triggered = true;
                // onBandTriggered[i].Invoke();
                VFXEventManager.InvokeBandTriggeredEvent(i);
            }
            else if (_bands[i].Intensity < _triggerThreshold && _bands[i].Triggered)
            {
                _bands[i].Triggered = false;
                //onBandReleased[i].Invoke();
                VFXEventManager.InvokeBandReleasedEvent(i);
            }
        }
    }

    public void SetSensitivity(float sensitivity)
    {
        _triggerThreshold = 0.01f + (sensitivity * 0.99f);
        
        Debug.Log($"Trigger Threshold: {_triggerThreshold}");
    }
}


