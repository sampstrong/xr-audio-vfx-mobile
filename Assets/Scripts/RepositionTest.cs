using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;

public class RepositionTest : AdjustableEffect
{
    [SerializeField] private GameObject _effectObject;
    
    private Vector3 _startPos;
    
    private Vector2 _touchStartPos;
    private Vector2 _touchEndPos;

    private void Start()
    {
        ResetStartPos();
    }

    private void Update()
    {
        Touch touch;
        
        switch (PlatformAgnosticInput.touchCount)
        {
            case 0:
                break; 
            case 1:
                Debug.Log("2");
                touch = PlatformAgnosticInput.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("3");
                    _touchStartPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Debug.Log("4");
                    _touchEndPos = touch.position;
                    var yDistance = _touchEndPos.y - _touchStartPos.y;

                    var adjustedDistance = (yDistance / Screen.height) * 4;

                    AdjustYPos(adjustedDistance);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    ResetStartPos();
                }
                break;
        }
    }

    public override void AdjustXPos(float xOffset)
    {
       
    }

    public override void AdjustYPos(float yOffset)
    {
        var newPos = _startPos + new Vector3(0, yOffset, 0);
        gameObject.transform.position = newPos;
        
        Debug.Log(newPos);
    }

    public override void AdjustZPos(float zOffset)
    {
       
    }

    public override void Recenter()
    {
        
    }

    public override void ResetStartPos()
    {
        _startPos = transform.position; 
    }
}
