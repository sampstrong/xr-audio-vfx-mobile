using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PixelNode : MonoBehaviour
{
    [SerializeField] private List<Renderer> _renderers;
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
        _pixelGlitch.VerticalOffsetStarted += ToggleVis;
        _pixelGlitch.VerticalOffsetEnded += ToggleVis;
        _nodeGroup.AllNodesCreated += CreateLines;
        _nodeGroup.AllNodesCreated += DrawLines;
        
        // CreateLines();
        // DrawLines();
    }

    private void ToggleVis()
    {
        if (_lines.Count <= 0) return;
        if (_lines[0].enabled)
        {
            foreach (var line in _lines)
                line.enabled = false;
        }
        else
        {
            foreach (var line in _lines)
                line.enabled = true;
        }
    }

    private void CreateLines()
    {
        Debug.Log($"Nodes Count: {_nodeGroup.Nodes.Count}");
        
        for (int i = 0; i < _nodeGroup.Nodes.Count; i++)
        {
            // if (_nodeGroup.Nodes[i].gameObject == this.gameObject) return;

            LineRenderer line = new GameObject($"Line {i}").AddComponent<LineRenderer>();
            line.transform.parent = gameObject.transform;
            //LineRenderer line = child.AddComponent<LineRenderer>();
            line.material = _lineMaterial;
            line.positionCount = 0;
            _lines.Add(line);
        }
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
            float distance = Vector3.Distance(transform.position, _pixelGlitch.Pixels[i].Obj.transform.position);
                
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
