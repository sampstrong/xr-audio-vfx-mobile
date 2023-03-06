using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class VerticalShaftGroup : MonoBehaviour
{
    public List<VerticalShaft> Shafts => _shafts;
    
    [SerializeField] private PixelGlitch _pixelGlitch;
    [SerializeField] private GameObject _shaftPrefab;

    private List<VerticalShaft> _shafts = new List<VerticalShaft>();
    
    void Awake()
    {
        _pixelGlitch.PixelsInitialized += Init;
    }

    private void Init()
    {
        for (int i = 0; i < _pixelGlitch.NumberOfPixels; i++)
        {
            var newShaft = Instantiate(_shaftPrefab, _pixelGlitch.CurrentPositions[i], Quaternion.identity, transform).GetComponent<VerticalShaft>();
            newShaft.Init(i, this, _pixelGlitch);
            _shafts.Add(newShaft);
        }
    }

   
}
