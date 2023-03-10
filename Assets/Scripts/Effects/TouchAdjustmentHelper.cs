using System.Collections;
using System.Collections.Generic;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;

public class TouchAdjustmentHelper : MonoBehaviour, IAdjustable
{
    private enum TouchDirection
    {
        X = 0,
        Y = 1
    }

    private TouchDirection _touchDirection;

    private bool _recenterEnabled = false;
    private bool _adjustmentEnabled = false;
    
    private Vector3 _objStartPos;
    private Vector2 _touchStartPos;
    private Vector2 _touchEndPos;

    private delegate void AdjustPosMethod(float offset);

    private AdjustPosMethod OffsetX;
    private AdjustPosMethod OffsetY;
    private AdjustPosMethod OffsetZ;
    
    void Start()
    {
        ResetStartPos();
        
        OffsetX = AdjustXPos;
        OffsetY = AdjustYPos;
        OffsetZ = AdjustZPos;
    }

    void Update()
    {
        if (!_adjustmentEnabled && !_recenterEnabled) return;
        
        if (_adjustmentEnabled)
        {
            switch (PlatformAgnosticInput.touchCount)
            {
                case 0:
                    break; 
                case 1:
                    GetOffsetFromTouch(0, TouchDirection.Y, OffsetY);
                    break;
                case 2:
                    GetOffsetFromTouch(1, TouchDirection.X, OffsetX);
                    break;
                case 3:
                    GetOffsetFromTouch(2, TouchDirection.Y, OffsetZ);
                    break;
                default:
                    break;
            }
        }
        else if (_recenterEnabled)
        {
            if (PlatformAgnosticInput.touchCount > 0)
            {
                var touch = PlatformAgnosticInput.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Recenter();
                }
            }
        }
    }

    private void GetOffsetFromTouch(
        int touchIndex, 
        TouchDirection direction, 
        AdjustPosMethod adjustmentMethod)
    {
        var touch = PlatformAgnosticInput.GetTouch(touchIndex);
        if (touch.phase == TouchPhase.Began)
        {
            _touchStartPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            _touchEndPos = touch.position;
            float distance;
            int unitsPerScreen;

            if (direction == TouchDirection.X)
            {
                distance = _touchEndPos.x - _touchStartPos.x;
                unitsPerScreen = 2;
            }
            else
            {
                distance = _touchEndPos.y - _touchStartPos.y;
                unitsPerScreen = 4;
            }
            
            var offset = (distance / Screen.width) * unitsPerScreen;
                    
            adjustmentMethod(offset);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            ResetStartPos();
        }
    }

    public void AdjustXPos(float xOffset)
    {
        var newPos = _objStartPos + new Vector3(xOffset, 0, 0);
        gameObject.transform.position = newPos;
    }

    public void AdjustYPos(float yOffset)
    {
        var newPos = _objStartPos + new Vector3(0, yOffset, 0);
        gameObject.transform.position = newPos;
    }

    public void AdjustZPos(float zOffset)
    {
        var newPos = _objStartPos + new Vector3(0, 0, zOffset);
        gameObject.transform.position = newPos;
    }

    public void Recenter()
    {
        Debug.Log("Recenter Triggered");
        
        if (Camera.main == null) return;
        var cam = Camera.main;
        var currentPos = transform.position;
        var distanceFromCam = Vector3.Distance(cam.transform.position, currentPos);

        var newPos = cam.transform.forward * distanceFromCam;
        newPos = new Vector3(newPos.x, currentPos.y, newPos.z);

        transform.position = newPos;
    }

    public void ResetStartPos()
    {
        _objStartPos = transform.position; 
    }

    public void ToggleAdjustmentEnabled()
    {
        _adjustmentEnabled = !_adjustmentEnabled;
    }

    public void ToggleRecenterEnabled()
    {
        _recenterEnabled = !_recenterEnabled;
    }
}
