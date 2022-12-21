using System.Collections;
using UnityEngine;

/// <summary>
/// Inherits from BandReceiver.
/// Creates semi-random rotations based on band trigger events.
/// </summary>
public class RandomRotator : BandReceiver
{
    [SerializeField] private float _minSpeed = 15;
    [SerializeField] private float _maxSpeed = 40;

    private float _currentSpeed;

    protected override void Start()
    {
        base.Start();

        _currentSpeed = _minSpeed;
    }
    
    void Update()
    {
        transform.Rotate(Vector3.up, _currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right, _currentSpeed / 5 * Time.deltaTime);
    }
    
    protected override void OnBandTriggered()
    {
        _currentSpeed = _maxSpeed;
    }

    protected override void OnBandReleased()
    {
        StartCoroutine(ReleaseWithDelay(1f));
    }

    private IEnumerator ReleaseWithDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _currentSpeed = _minSpeed;

    }
    
    
}
