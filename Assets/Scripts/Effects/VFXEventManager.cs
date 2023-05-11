using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static event manager for all events that VFX respond to
/// These could be triggered from audio data or manual controls
/// </summary>
public static class VFXEventManager
{
    public static event Action<int> onBandTriggered;
    public static event Action<int> onBandReleased;

    public static event Action<int> onBar;

    public static event Action<int> onHalfBar;
    
    public static event Action BaseStarted;
    public static event Action BuildStarted;
    public static event Action DropStarted;
    public static event Action BreakStarted;

    public static int[] beatCounter = new int[8];
    
    
    public static void InvokeBandTriggeredEvent(int band)
    {
        onBandTriggered?.Invoke(band);
        CountBeats(band);
    }
    
    public static void InvokeBandReleasedEvent(int band)
    {
        onBandReleased?.Invoke(band);
    }

    public static void InvokeBaseStartedEvent()
    {
        BaseStarted?.Invoke();
    }

    public static void InvokeBuildStartedEvent()
    {
        BuildStarted?.Invoke();
    }

    public static void InvokeDropStartedEvent()
    {
        DropStarted?.Invoke();
    }

    public static void InvokeBreakStartedEvent()
    {
        BreakStarted?.Invoke();
    }

    private static void CountBeats(int band)
    {
        beatCounter[band] += 1;
        
        if (band == 1)
        {
            // Debug.Log($"Beat count: {beatCounter[band]}");
        }

        if (beatCounter[band] == 2)
        {
            onHalfBar?.Invoke(band);
            if (band == 1)
            {
                // Debug.Log("On Half Bar Invoked");
            }
        }
        else if (beatCounter[band] == 4)
        {
            onBar?.Invoke(band);
            beatCounter[band] = 0;
            if (band == 1)
            {
                // Debug.Log("On Bar Invoked");
            }
        }
        else if (beatCounter[band] > 4)
        {
            beatCounter[band] = 0;
        }
    }
}
