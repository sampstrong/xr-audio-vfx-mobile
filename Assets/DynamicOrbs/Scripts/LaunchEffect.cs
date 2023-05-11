using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LaunchEffect : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private VisualEffect[] _particleEffects;
    
    private Vector3 _targetPos;

    public void Init(Vector3 targetPos)
    {
        _targetPos = targetPos;
        _renderer.enabled = true;
        foreach (var e in _particleEffects)
        {
            e.gameObject.SetActive(true);
        }
    }

    public void Disable()
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
}
