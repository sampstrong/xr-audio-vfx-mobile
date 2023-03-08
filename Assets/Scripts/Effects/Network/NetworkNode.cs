using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class NetworkNode : NetworkObject
{

    public override void Init(int index, NetworkGroup group, NetworkController controller)
    {
        _index = index;
        _networkGroup = group;
        _networkController = controller;
        _networkFollower.Init(index, group, controller);
    }

    protected override void InitBaseState()
    {
        ControlVis(VisibilityState.On);
    }

    protected override void RunBaseState()
    {
        
    }

    protected override void InitBuildState()
    {
        ControlVis(VisibilityState.On);
    }

    protected override void RunBuildState()
    {
        
    }

    protected override void InitDropState()
    {
        
    }

    protected override void RunDropState()
    {
        
    }

    protected override void InitBreakState()
    {
        ControlVis(VisibilityState.On);
    }

    protected override void RunBreakState()
    {
        
    }
}
