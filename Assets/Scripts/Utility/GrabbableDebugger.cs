using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableDebugger : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    public void SetMaterial(Material material)
    {
        _renderer.material = material;
    }

    public void DebugEvent(string logText)
    {
        Debug.Log($"Grabbable: {logText}");
    }
}
