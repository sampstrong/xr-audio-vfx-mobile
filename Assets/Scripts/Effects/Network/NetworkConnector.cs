using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(NetworkFollower))]
public class NetworkConnector : NetworkObject
{
    [SerializeField] private List<Renderer> _renderers;
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _distanceThreshold;
    [SerializeField] private float _minLineWidth;
    [SerializeField] private float _maxLineWidth;

    private NetworkFollower _networkFollower;
    private NetworkGroup _networkGroup;
    private NetworkController _networkController;
    private int _index;

    private List<LineRenderer> _lines = new List<LineRenderer>();

    public override void Init(int index, NetworkGroup nodeGroup, NetworkController networkController)
    {
        _index = index;
        _networkGroup = nodeGroup;
        _networkController = networkController;

        _networkController.PositionUpdated += UpdateLines;
        _networkController.VerticalOffsetStarted += ToggleVis;
        _networkController.VerticalOffsetEnded += ToggleVis;
        _networkGroup.AllObjectsCreated += CreateLines;
        _networkGroup.AllObjectsCreated += DrawLines;
        
    }

    public override void ControlVis(VisibilityState state)
    {
        
    }

    public override void Strobe(StrobeState state)
    {
        
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
        for (int i = 0; i < _networkGroup.NetworkObjects.Count; i++)
        {
            LineRenderer line = new GameObject($"Line {i}").AddComponent<LineRenderer>();
            line.transform.parent = gameObject.transform;
            line.material = _lineMaterial;
            line.positionCount = 0;
            _lines.Add(line);
        }
    }

    private void UpdateLines(int index, Vector3 pos)
    {
        if (index != _index) return;
        transform.position = pos;
        DrawLines();
    }

    private void DrawLines()
    {
        // sets up line renderers to connect to other nearby nodes when they are closer than the threshold
        for (int i = 0; i < _networkGroup.NetworkObjects.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, _networkGroup.NetworkObjects[i].transform.position);
                
            if (distance < _distanceThreshold)
            {
                _lines[i].positionCount = 2;
                _lines[i].SetPosition(0, transform.position);
                _lines[i].SetPosition(1, _networkGroup.NetworkObjects[i].transform.position);
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
