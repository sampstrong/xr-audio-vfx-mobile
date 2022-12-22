using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _effects;
    [SerializeField] private SpatialAnchorManager _anchorManager;

    private GameObject _currentEffect;
    
    public void SetCurrentEffect(int index)
    {
        _currentEffect = _effects[index];
        Debug.Log($"Current Effect Set To: {_effects[index].name}");
    }

    public void PlaceEffect()
    {
        _anchorManager.PlaceAnchor(_currentEffect);
    }
}
