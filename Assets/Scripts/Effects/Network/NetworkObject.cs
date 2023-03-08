using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(NetworkFollower))]
public abstract class NetworkObject : MonoBehaviour
{
    [SerializeField] protected NetworkFollower _networkFollower;
    [SerializeField] private List<Renderer> _renderers;
    
    protected NetworkGroup _networkGroup;
    protected NetworkController _networkController;
    protected int _index;

    public enum VisibilityState
    {
        Off = 0,
        On = 1
    }
    
    public VisibilityState _visibilityState { get; set; }

    protected virtual void Start()
    {
        Assert.IsNotNull(_networkFollower, "Please Assign NetworkFollower component in the Inspector");
        VFXEventManager.BaseStarted += InitBaseState;
        VFXEventManager.BuildStarted += InitBuildState;
        VFXEventManager.DropStarted += InitDropState;
        VFXEventManager.BreakStarted += InitBreakState;
    }

    public abstract void Init(int index, NetworkGroup group, NetworkController controller);

    protected virtual void Update()
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
    
    public virtual void ControlVis(VisibilityState state)
    {
        if (state == VisibilityState.On)
        {
            foreach (var rend in _renderers)
            {
                rend.enabled = true;
            }
        }
        else if (state == VisibilityState.Off)
        {
            foreach (var rend in _renderers)
            {
                rend.enabled = false;
            }
        }
    }

    protected abstract void InitBaseState();

    protected abstract void RunBaseState();

    protected abstract void InitBuildState();

    protected abstract void RunBuildState();

    protected abstract void InitDropState();

    protected abstract void RunDropState();

    protected abstract void InitBreakState();

    protected abstract void RunBreakState();

}
