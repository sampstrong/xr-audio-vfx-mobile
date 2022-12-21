using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Inherits from AudioController. Used to control velocity of objects based on audio input
/// </summary>
public class AudioVelocityController : AudioController
{
    [SerializeField] private NodeGroup _nodeGroup;
    [SerializeField] private float _speedMin = 1f;
    [SerializeField] private float _speedMax = 1.001f;
    [SerializeField] private Material _matOne;
    [SerializeField] private Material _matTwo;

    private bool _aboveTriggerThreshold = false;
    private float _currentSpeed;

    private UnityEvent onKickTriggered;
    private UnityEvent onKickReleased;

    private int _beatCount = 0;

    private void Start()
    {
        ChangeMaterial(_matOne);
    }
    
    protected override void Update()
    {
        base.Update();
        GetBeatTrigger();
    }
    
    /// <summary>
    /// Increases velocity and changes material when the specified band goes above the threshold.
    /// Changes back to original velocity and material when the band goes below the threshold.
    /// </summary>
    private void GetBeatTrigger()
    {
        if (_audioBandIntensityBuffer > 0.5f && !_aboveTriggerThreshold)
        {
            Debug.Log("Beat Triggered");
            _aboveTriggerThreshold = true;
            IncreaseVelocity();
            ChangeMaterial(_matTwo);
            _beatCount++;
            if (_beatCount >= 4)
            {
                foreach (var node in _nodeGroup.Nodes)
                {
                    node.rb.velocity = _nodeGroup.SetVelocity();  
                }

                _beatCount = 0;
            }
        }
        else if (_audioBandIntensityBuffer < 0.5 && _aboveTriggerThreshold)
        {
            _aboveTriggerThreshold = false;
            DecreaseVelocity();
            ChangeMaterial(_matOne);
        }
    }

    /// <summary>
    /// Increases the velocity of each node in the group
    /// </summary>
    private void IncreaseVelocity()
    {
        if (_nodeGroup.Nodes.Count < _nodeGroup.NumberOfNodes) return;

        foreach (var node in _nodeGroup.Nodes)
        {
            _currentSpeed = SetSpeed(_speedMin, _speedMax);
            node.rb.velocity *= _currentSpeed;
        }
    }

    /// <summary>
    /// Decreases the velocity of each node in the group
    /// </summary>
    private void DecreaseVelocity()
    {
        if (_nodeGroup.Nodes.Count < _nodeGroup.NumberOfNodes) return;

        foreach (var node in _nodeGroup.Nodes)
        {
            node.rb.velocity /= _currentSpeed;
        }
    }

    /// <summary>
    /// Sets the speed based on the audio band's intensity
    /// </summary>
    /// <param name="min"></param>
    /// Minimum speed the nodes will travel
    /// <param name="max"></param>
    /// Maximum speed the nodes will travel
    /// <returns></returns>
    private float SetSpeed(float min, float max)
    {
        float speed = GetControlValue(_audioBandIntensityBuffer, min, max);
        return speed;
    }

    /// <summary>
    /// Changes the material for each node
    /// </summary>
    /// <param name="mat"></param>
    private void ChangeMaterial(Material mat)
    {
        foreach (var node in _nodeGroup.Nodes)
        {
            node.rend.material = mat;
        }
    }
}
