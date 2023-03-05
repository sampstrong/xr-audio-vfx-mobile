using System;

/// <summary>
/// Static event manager for all events that VFX respond to
/// These could be triggered from audio data or manual controls
/// </summary>
public static class VFXEventManager
{
    public static event Action<int> onBandTriggered;
    public static event Action<int> onBandReleased;
    
    public static void InvokeBandTriggeredEvent(int band)
    {
        onBandTriggered?.Invoke(band);
    }
    
    public static void InvokeBandReleasedEvent(int band)
    {
        onBandReleased?.Invoke(band);
    }

    public static void InvokeTransitionEvent()
    {
        
    }

    public static void InvokeDropEvent()
    {
        
    }
}
