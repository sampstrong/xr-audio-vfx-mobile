using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class NetworkPixel : NetworkObject
{
    [SerializeField] private float _strobeSpeed = 10.0f;
    [SerializeField] private int _band = 1;
    [SerializeField] private float _strobeDuration = 0.5f;

    private bool _strobing = false;

    public override void Init(int index, NetworkGroup group, NetworkController controller)
    {
        _index = index;
        _networkGroup = group;
        _networkController = controller;
        _networkFollower.Init(index, group, controller);
    }

    protected override void InitBaseState()
    {
        VFXEventManager.onHalfBar -= FilterHalfBarTrigger;
        ControlVis(VisibilityState.On);
    }

    protected override void RunBaseState()
    {
        
    }

    protected override void InitBuildState()
    {
        VFXEventManager.onHalfBar -= FilterHalfBarTrigger;
        ControlVis(VisibilityState.Off);
    }

    protected override void RunBuildState()
    {
        ControlVis(VisibilityState.On);
        SynchronizedStrobe();
    }

    protected override void InitDropState()
    {
        VFXEventManager.onHalfBar += FilterHalfBarTrigger;
        ControlVis(VisibilityState.On);
    }

    protected override void RunDropState()
    {
        
    }

    protected override void InitBreakState()
    {
        VFXEventManager.onHalfBar -= FilterHalfBarTrigger;
        ControlVis(VisibilityState.Off);
    }

    protected override void RunBreakState()
    {
        
    }

    private void FilterHalfBarTrigger(int band)
    {
        if (band != _band) return;

        if (!_strobing)
        {
            StartCoroutine(TimedStrobe());
        }
    }

    private IEnumerator TimedStrobe()
    {
        _strobing = true;
        float t = 0.0f;

        while (t < _strobeDuration)
        {
            t += Time.deltaTime;
            var triggerValue = Mathf.Sin(Time.time * _strobeSpeed);

            if (triggerValue > 0.5f)
                ControlVis(VisibilityState.On);
            else if (triggerValue < -0.5f)
                ControlVis(VisibilityState.Off);

            yield return null;
        }
        
        ControlVis(VisibilityState.On);

        _strobing = false;
    }

    private void SynchronizedStrobe()
    {
        var triggerValue = Mathf.Sin(Time.time * _strobeSpeed / 4.0f);

        if (triggerValue >= -0.5)
            ControlVis(VisibilityState.Off);
        else if (triggerValue <= 0.5)
            ControlVis(VisibilityState.On);
    }
}
