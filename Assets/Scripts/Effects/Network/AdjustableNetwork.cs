using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableNetwork : AdjustableEffect
{
    [SerializeField] private GameObject _effectObject;

    private Vector3 _startPos;

    private RepositionTest _test;

    private void Start()
    {
        ResetStartPos();

        
    }
    
    public override void AdjustXPos(float xOffset)
    {
        var currentPos = _effectObject.transform.position;
        var newPos = new Vector3(xOffset, currentPos.y, currentPos.z);

        _effectObject.transform.position = newPos;
    }

    public override void AdjustYPos(float yOffset)
    {
        //var newPos = new Vector3(_startPos.x, _startPos.y + yOffset, _startPos.z);
        //
        //Debug.Log($"New Pos: {newPos}");
//
        //_effectObject.transform.localPosition = newPos;
        //
        //Debug.Log($"{_effectObject.name} position: {_effectObject.transform.localPosition}");

        var newPos = _startPos + new Vector3(0, yOffset, 0);
        _effectObject.gameObject.transform.position = newPos;
        
        _test = FindObjectOfType<RepositionTest>();
        _test.transform.position = newPos;
        
        Debug.Log(newPos);
    }

    public override void AdjustZPos(float zOffset)
    {
        var currentPos = _effectObject.transform.position;
        var newPos = new Vector3(currentPos.x, currentPos.y, zOffset);

        _effectObject.transform.position = newPos;
    }

    public override void ResetStartPos()
    {
        _startPos = _effectObject.transform.localPosition;
    }

    public override void Recenter()
    {
        
    }

    private void TriggerPosUpdated()
    {
        
    }
}
