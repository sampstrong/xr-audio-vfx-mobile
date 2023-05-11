using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchEffect : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    
    private Vector3 _targetPos;

    public void Init(Vector3 targetPos)
    {
        _targetPos = targetPos;
        _renderer.enabled = true;
    }

    public void Disable()
    {
        _renderer.enabled = false;
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

    // void Update()
    // {
    //     transform.position = _targetPos;
    // }
}
