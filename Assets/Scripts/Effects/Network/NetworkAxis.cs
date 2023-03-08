using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkFollower))]
public class NetworkAxis : NetworkObject
{
    [SerializeField] private NetworkFollower _networkFollower;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _strobeSpeed = 10.0f;
    
    private NetworkGroup _networkGroup;
    private NetworkController _networkController;
    private int _index;
    
    private float _randomOffset;
    private float _rotSpeed1;
    private float _rotSpeed2;

    public void Start()
    {
        Assert.IsNotNull(_networkFollower, "Please Assign NetworkFollower component in the Inspector");
        EffectManager.Instance.BaseStarted += InitBaseState;
        EffectManager.Instance.DropStarted += InitDropState;
    }

    public override void Init(int index, NetworkGroup group, NetworkController controller)
    {
        _index = index;
        _networkGroup = group;
        _networkController = controller;
        _networkFollower.Init(index, group, controller);
        _randomOffset = Random.Range(50, 150);
        
        InitBaseState();
    }

    private void Update()
    {
        switch (EffectManager.Instance.CurrentPreset)
        {
            case EffectManager.Preset.Base:
                RunBaseState();
                break;
            case EffectManager.Preset.Build:
                RunBuildState();
                break;
            case EffectManager.Preset.Drop:
                RunDropState();
                break;
            case EffectManager.Preset.Break:
                RunBreakState();
                break;
            default:
                RunBaseState();
                break;
        }
    }

    private void InitBaseState()
    {
        transform.rotation = quaternion.Euler(0,0,0);
        
        var rand = Random.value;
        
        if (rand >= 0.5f)
        {
            _renderer.enabled = false;
        }
    }

    private void RunBaseState()
    {
        
    }

    private void RunBuildState()
    {
        
    }

    private void InitDropState()
    {
        _rotSpeed1 = _randomOffset / 10.0f;
        _rotSpeed2 = _randomOffset / 5.0f;
        
        var randX = Random.Range(0, 360);
        var randY = Random.Range(0, 360);
        var randZ = Random.Range(0, 360);

        transform.rotation = Quaternion.Euler(randX, randY, randZ);
    }

    private void RunDropState()
    {
        Strobe(StrobeState.Randomized);
        RandomRotate();
    }

    private void RunBreakState()
    {
        RandomRotate();
    }

    public override void ControlVis(VisibilityState state)
    {
        
    }

    public override void Strobe(StrobeState state)
    {
        var triggerValue = Mathf.Sin((Time.time + _randomOffset) * _strobeSpeed);

        if (triggerValue >= -0.5)
            _renderer.enabled = false;
        else if (triggerValue <= 0.5)
            _renderer.enabled = true;
    }

    private void RandomRotate()
    {
        transform.Rotate(Time.deltaTime * _rotSpeed1, 0, Time.deltaTime * _rotSpeed2);
    }
}
