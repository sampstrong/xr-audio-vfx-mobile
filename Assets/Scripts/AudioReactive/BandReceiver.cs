using UnityEngine;

/// <summary>
/// Base class for receiving band triggered and released events
/// Inherit from this class to receive events and set up the desired
/// behavior in OnBandTriggered() and OnBandReleased()
/// </summary>
public abstract class BandReceiver : MonoBehaviour
{
    [SerializeField] protected int _band;
    
    protected virtual void Start()
    {
        BandTrigger.Instance.onBandTriggered[_band].AddListener(OnBandTriggered);
        BandTrigger.Instance.onBandTriggered[_band].AddListener(OnBandReleased);
    }

    protected abstract void OnBandTriggered();

    protected abstract void OnBandReleased();
}
