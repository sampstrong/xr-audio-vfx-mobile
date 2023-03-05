using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Creates global trigger events for each frequency band when they get above a certain intensity
/// </summary>
public class BandTrigger : MonoBehaviour
{
    public Band[] Bands { get => _bands; }
    
    // events for going above and below threshold
    // public UnityEvent[] onBandTriggered = new UnityEvent[8];
    // public UnityEvent[] onBandReleased = new UnityEvent[8];
    
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
            if (_bands[i].Intensity > 0.5f && !_bands[i].Triggered)
            {
                _bands[i].Triggered = true;
                // onBandTriggered[i].Invoke();
                VFXEventManager.InvokeBandTriggeredEvent(i);
            }
            else if (_bands[i].Intensity < 0.5 && _bands[i].Triggered)
            {
                _bands[i].Triggered = false;
                //onBandReleased[i].Invoke();
                VFXEventManager.InvokeBandReleasedEvent(i);
            }
        }
    }
}


