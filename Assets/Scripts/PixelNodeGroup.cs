using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PixelNodeGroup : MonoBehaviour
{
    public List<PixelNode> Nodes => _nodes;
    
    [SerializeField] private PixelGlitch _pixelGlitch;
    [SerializeField] private GameObject _nodePrefab;

    private List<PixelNode> _nodes = new List<PixelNode>();
    
    // Must subscribe in Awake because event is occuring in Start of PixelGlitch
    void Awake()
    {
        _pixelGlitch.PixelsInitialized += Init;
    }

    void Update()
    {
        
    }

    private void Init()
    {
        for (int i = 0; i < _pixelGlitch.NumberOfPixels; i++)
        {
            var newNode = Instantiate(_nodePrefab, _pixelGlitch.CurrentPositions[i], Quaternion.identity).GetComponent<PixelNode>();
            newNode.Init(i, this, _pixelGlitch);
            _nodes.Add(newNode);
        }
    }
}
