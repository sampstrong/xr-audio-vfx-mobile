using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

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
    

    public void Start()
    {
        Assert.IsNotNull(_networkFollower, "Please Assign NetworkFollower component in the Inspector");
    }

    public override void Init(int index, NetworkGroup group, NetworkController controller)
    {
        _index = index;
        _networkGroup = group;
        _networkController = controller;
        _networkFollower.Init(index, group, controller);
        _randomOffset = Random.Range(50, 150);
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

    private void RunBaseState()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    private void RunBuildState()
    {
        
    }

    private void RunDropState()
    {
        Strobe(StrobeState.Randomized);
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
        var speed1 = _randomOffset / 10.0f;
        var speed2 = _randomOffset / 5.0f;

        transform.Rotate(Vector3.forward, Time.deltaTime * speed1);
        transform.Rotate(Vector3.right, Time.deltaTime * speed2);
    }
}
