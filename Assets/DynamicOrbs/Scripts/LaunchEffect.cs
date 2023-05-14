using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.VFX;

public class LaunchEffect : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private VisualEffect[] _particleEffects;
    [SerializeField] private float _offset = 0.25f;
    private Vector3 _targetPos;
    

    private void Start()
    {
        TouchManager.TouchStarted += Init;
        TouchManager.TouchHappened += HandleTouch;
        TouchManager.TouchEnded += Disable;
    }

    private void HandleTouch(Touch touch, TouchManager.TouchZone zone)
    {
        if (zone != TouchManager.TouchZone.World) return;
        if (OrbsGroup.InteractionState == OrbsGroup.OrbInteractionState.Play) return;
        
        var targetPos = ServiceLocator.Instance.ARCamera.ScreenToWorldPoint(
            new Vector3(touch.position.x, touch.position.y, _offset));
        
        SetTargetPos(targetPos);
    }

    
    public void Init(Touch touch, TouchManager.TouchZone zone)
    {
        if (zone != TouchManager.TouchZone.World) return;
        if (OrbsGroup.InteractionState == OrbsGroup.OrbInteractionState.Play) return;
        
        _renderer.enabled = true;
        foreach (var e in _particleEffects)
        {
            e.gameObject.SetActive(true);
        }
    }

    public void Disable(Touch touch, TouchManager.TouchZone zone)
    {
        _renderer.enabled = false;
        foreach (var e in _particleEffects)
        {
            e.gameObject.SetActive(false);
        }
    }

    private IEnumerator PopIn()
    {
        yield return null;
    }
    
    private IEnumerator PopOut()
    {
        yield return null;
    }

    public void SetTargetPos(Vector3 pos)
    {
        _targetPos = pos;
        transform.position = _targetPos;
    }

    void Update()
    {
        transform.LookAt(ServiceLocator.Instance.ARCamera.transform);
    }

    public void SetColor(Color color)
    {
        foreach (var e in _particleEffects)
        {
            e.SetVector4("_Color", color);
        }
    }

    private void OnDisable()
    {
        EndProcesses();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            EndProcesses();
    }

    private void EndProcesses()
    {
        TouchManager.TouchStarted -= Init;
        TouchManager.TouchHappened -= HandleTouch;
        TouchManager.TouchEnded -= Disable;
    }
}
