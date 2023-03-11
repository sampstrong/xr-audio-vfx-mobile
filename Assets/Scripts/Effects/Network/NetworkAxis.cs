using System.Collections;
using System.Collections.Generic;
using Niantic.ARDK.Utilities.Input.Legacy;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class NetworkAxis : NetworkObject
{
    [SerializeField] private float _strobeSpeed = 10.0f;

    private float _randomOffset;
    private float _rotSpeed1;
    private float _rotSpeed2;
    
    public enum StrobeState
    {
        None = 0,
        Synchronized = 1,
        Randomized = 2,
    }

    private StrobeState _strobeState { get; set; }

    public override void Init(int index, NetworkGroup group, NetworkController controller)
    {
        _index = index;
        _networkGroup = group;
        _networkController = controller;
        _networkFollower.Init(index, group, controller);
        _randomOffset = Random.Range(50, 150);
        
        InitBaseState();
        
        _rotSpeed1 = _randomOffset / 15.0f;
        _rotSpeed2 = _randomOffset / 7.0f;
    }
    
    protected override void Update()
    {
        base.Update();

        if (ServiceLocator.Instance.EffectManager.AdjustmentEnabled) return;
        
        
        if (PlatformAgnosticInput.touchCount <= 0) return;
        var touch = PlatformAgnosticInput.GetTouch(0);
        
        var noTouchZone = Screen.height / 8;
        if (touch.position.y < noTouchZone || touch.position.y > Screen.height - noTouchZone)
            return;

        switch (PlatformAgnosticInput.touchCount)
        {
            case 0:
                break;
            case 1:
                Strobe(StrobeState.Randomized);
                break;
            case 2:
                Strobe(StrobeState.Synchronized);
                break;
            default:
                break;
        }

        // return to previous state
        if (touch.phase == TouchPhase.Ended)
        {
            switch (ServiceLocator.Instance.EffectManager.CurrentPreset)
            {
                case EffectManager.Preset.Base:
                    ControlVis(VisibilityState.Off);
                    break;
                case EffectManager.Preset.Build:
                    ControlVis(VisibilityState.On);
                    break;
                case EffectManager.Preset.Drop:
                    ControlVis(VisibilityState.On);
                    var rand = Random.value;
        
                    if (rand >= 0.5f)
                    {
                        ControlVis(VisibilityState.Off);
                    }
                    break;
                case EffectManager.Preset.Break:
                    ControlVis(VisibilityState.On);
                    break;
            }
        }
    }

    protected override void InitBaseState()
    {
        transform.rotation = quaternion.Euler(0,0,0);
        
        ControlVis(VisibilityState.Off);
    }
    

    protected override void RunBaseState()
    {
        
    }

    protected override void InitBuildState()
    {
        ControlVis(VisibilityState.On);
        
        if (transform.rotation.x == 0)
            SetRandRot();
    }

    protected override void RunBuildState()
    {
        RandomRotate();
    }

    protected override void InitDropState()
    {
        ControlVis(VisibilityState.On);
        
        transform.rotation = quaternion.Euler(0,0,0);
        var rand = Random.value;
        
        if (rand >= 0.5f)
        {
            ControlVis(VisibilityState.Off);
        }
    }

    protected override void RunDropState()
    {
        // Set to touch only
        // Strobe(StrobeState.Randomized);
    }

    protected override void InitBreakState()
    {
        ControlVis(VisibilityState.On);
        
        
        
        if (transform.rotation.x == 0)
            SetRandRot();
    }

    protected override void RunBreakState()
    {
        RandomRotate();
    }

    private void SetRandRot()
    {
        var randX = Random.Range(0, 360);
        var randY = Random.Range(0, 360);
        var randZ = Random.Range(0, 360);

        transform.rotation = Quaternion.Euler(randX, randY, randZ);
    }

    private void Strobe(StrobeState state)
    {
        float offset;

        if (state == StrobeState.Randomized)
            offset = _randomOffset;
        else
            offset = 0;

        var triggerValue = Mathf.Sin((Time.time + offset) * _strobeSpeed);

        if (triggerValue >= -0.5)
            ControlVis(VisibilityState.Off);
        else if (triggerValue <= 0.5)
            ControlVis(VisibilityState.On);
    }

    private void RandomRotate()
    {
        transform.Rotate(Time.deltaTime * _rotSpeed1, 0, Time.deltaTime * _rotSpeed2);
    }
}
