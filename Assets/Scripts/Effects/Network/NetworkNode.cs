using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

[RequireComponent(typeof(NetworkFollower))]
public class NetworkNode : NetworkObject
{
    [SerializeField] private NetworkFollower _networkFollower;
    
    private NetworkGroup _networkGroup;
    private NetworkController _networkController;
    private int _index;

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
    }

    public override void ControlVis(VisibilityState state)
    {
        
    }

    public override void Strobe(StrobeState state)
    {
        
    }
}
