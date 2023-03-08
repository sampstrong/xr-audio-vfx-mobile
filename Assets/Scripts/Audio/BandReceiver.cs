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
        VFXEventManager.onBandTriggered += OnBandTriggered;
        VFXEventManager.onBandTriggered += OnBandReleased;
    }

    protected abstract void OnBandTriggered(int band);

    protected abstract void OnBandReleased(int band);
}
