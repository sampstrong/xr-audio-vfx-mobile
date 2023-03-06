using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PixelNode : MonoBehaviour
{
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _distanceThreshold;
    [SerializeField] private float _minLineWidth;
    [SerializeField] private float _maxLineWidth;

    private PixelNodeGroup _nodeGroup;
    private PixelGlitch _pixelGlitch;
    private int _nodeIndex;

    private List<LineRenderer> _lines = new List<LineRenderer>();

    public void Init(int index, PixelNodeGroup nodeGroup, PixelGlitch pixelGlitch)
    {
        _nodeIndex = index;
        _nodeGroup = nodeGroup;
        _pixelGlitch = pixelGlitch;
        _pixelGlitch.PositionUpdated += UpdateNodePosition;
        
        CreateLines();
        DrawLines();
    }

    private void CreateLines()
    {
        for (int i = 0; i < _nodeGroup.Nodes.Count; i++)
        {
            if (_nodeGroup.Nodes[i] == this) return;

            GameObject child = Instantiate(new GameObject($"Line {i}"), transform);
            LineRenderer line = child.AddComponent<LineRenderer>();
            line.material = _lineMaterial;
            line.positionCount = 0;
            _lines.Add(line);
        }
    }

    private void Update()
    {
        
    }
    
    private void UpdateNodePosition(int index, Vector3 pos)
    {
        if (index != _nodeIndex) return;
        transform.position = pos;
        DrawLines();
    }
    
    private void DrawLines()
    {
        // sets up line renderers to connect to other nearby nodes when they are closer than the threshold
        for (int i = 0; i < _nodeGroup.Nodes.Count; i++)
        {
            if (_nodeGroup.Nodes[i] == this) return;
            float distance = Vector3.Distance(transform.position, _nodeGroup.Nodes[i].transform.position);
                
            if (distance < _distanceThreshold)
            {
                _lines[i].positionCount = 2;
                _lines[i].SetPosition(0, transform.position);
                _lines[i].SetPosition(1, _nodeGroup.Nodes[i].transform.position);
                _lines[i].startWidth = 1 / distance.Remap(0, 3, _minLineWidth, _maxLineWidth);
                _lines[i].endWidth = 1 / distance.Remap(0, 3, _minLineWidth, _maxLineWidth);
            }
            else
            {
                _lines[i].positionCount = 0;
            }
        }
    }
}
