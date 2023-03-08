using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NetworkFollower : MonoBehaviour
{
    private NetworkGroup _networkGroup;
    private NetworkController _networkController;
    private int _index;

    private enum FollowType
    {
        PositionAndScale = 0,
        PositionOnly = 1,
        MainPositionsOnly = 2,
    }

    [SerializeField] private FollowType _followType;

    public void Init(int index, NetworkGroup group, NetworkController controller)
    {
        _index = index;
        _networkGroup = group;
        _networkController = controller;
        _networkController.PositionUpdated += UpdatePosition;
        _networkController.ScaleUpdated += UpdateScale;
    }
    
    private void UpdatePosition(int index, Vector3 pos)
    {
        if (_followType is FollowType.MainPositionsOnly &&
            _networkController.CurrentAnimationState is NetworkController.AnimationState.VerticalOffset)
            return;
        if (index != _index) 
            return;
        transform.position = pos; // local position?
        Debug.Log($"Current Pos Index 0: {_networkController.CurrentPositions[0]}");
    }

    private void UpdateScale(int index, Vector3 scale)
    {
        if (_followType is FollowType.PositionOnly or FollowType.MainPositionsOnly) 
            return;
        if (index != _index) 
            return;
        transform.localScale = scale;
    }
}
