using System.Collections;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public Renderer Renderer => _renderer;
    public OrbState CurrentOrbState => _orbState;
    public OrbFrequency OrbFrequency => _orbFrequency;
    public float Intensity => _intensity;

    public enum OrbState
    {
        Disabled = 0,
        Enabled = 1,
        PoppingIn = 2,
        PoppingOut = 3
    }

    private OrbState _orbState;
    private OrbFrequency _orbFrequency;
    private float _intensity;

    [Header("Component References")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private Rigidbody _rigidBody;
    
    [Header("Movement Properties")]
    [SerializeField] private float _xBounds = 0.5f;
    [SerializeField] private float _yBounds = 0.5f;
    [SerializeField] private float _zBounds = 0.5f;
    [SerializeField] private float _velocityMultiplier = 0.1f;
    [SerializeField] private float _minVelocity = 0.05f;
    [SerializeField] private float _minScale = 0.2f;
    [SerializeField] private float _maxScale = 1f;

    private float _localBoundsLowerX;
    private float _localBoundsUpperX;
    private float _localBoundsLowerY;
    private float _localBoundsUpperY;
    private float _localBoundsLowerZ;
    private float _localBoundsUpperZ;

    private float _popInDuration = 1f;
    private Vector3 _startingVelocity;
    private Vector3 _origin;
    private Vector3 _baseScale;

    private void Start()
    {
        _origin = new Vector3(0, 0, 0);
        _baseScale = new Vector3(1, 1, 1);
        _rigidBody.velocity = HelperMethods.GetRandomVec3() * _velocityMultiplier;
    }
    
    private void Update()
    {
        if (_orbState == OrbState.Disabled) return;
        UpdateIntensity();
        UpdateRigidbodies();
    }

    public void InitOrb(Vector3 origin, Vector3 position, Vector3 velocity, OrbFrequency freq)
    {
        StartCoroutine(PopIn());
        _renderer.enabled = true;
        transform.position = position;
        _rigidBody.velocity = velocity;
        _orbFrequency = freq;
        _origin = origin;
        SetLocalBounds(_origin);
    }
    
    public void DisableOrb()
    {
        _orbState = OrbState.Disabled;
        _renderer.enabled = false;
        transform.position = new Vector3(0, 100, 0);
    }

    private IEnumerator PopIn()
    {
        _orbState = OrbState.PoppingIn;

        for (float t = 0; t < _popInDuration; t += Time.deltaTime)
        {
            var scale = Mathf.Lerp(0f, 1f, t / _popInDuration);
            transform.localScale = new Vector3(scale, scale, scale);
            
            yield return null;
        }
        
        transform.localScale = Vector3.one;
        _orbState = OrbState.Enabled;
    }

    private void UpdateIntensity()
    {
        _intensity = AudioSpectrumReader.audioBandIntensityBuffer[_orbFrequency.band];
    }

    private void SetLocalBounds(Vector3 origin)
    {
        _localBoundsLowerX = origin.x - _xBounds;
        _localBoundsUpperX = origin.x + _xBounds;
        _localBoundsLowerY = origin.y - _yBounds;
        _localBoundsUpperY = origin.y + _yBounds;
        _localBoundsLowerZ = origin.z - _zBounds;
        _localBoundsUpperZ = origin.z + _zBounds;
    }


    private void UpdateRigidbodies()
    {
        var p = _rigidBody.transform.position;
        var v = _rigidBody.velocity;

        if (v.magnitude < _minVelocity)
        {
            _rigidBody.velocity *= 1.1f;
        }

        if (_rigidBody.transform.position.x > _localBoundsUpperX)
        {
            _rigidBody.transform.position = new Vector3(_localBoundsUpperX, p.y, p.z);
            _rigidBody.velocity = new Vector3(-v.x, v.y, v.z);
        }
        else if (_rigidBody.transform.position.x < _localBoundsLowerX)
        {
            _rigidBody.transform.position = new Vector3(_localBoundsLowerX, p.y, p.z);
            _rigidBody.velocity = new Vector3(-v.x, v.y, v.z);
        }

        if (_rigidBody.transform.position.y > _localBoundsUpperY)
        {
            _rigidBody.transform.position = new Vector3(p.x, _localBoundsUpperY, p.z);
            _rigidBody.velocity = new Vector3(v.x, -v.y, v.z);
        }
        else if (_rigidBody.transform.position.y < _localBoundsLowerY)
        {
            _rigidBody.transform.position = new Vector3(p.x, _localBoundsLowerY, p.z);
            _rigidBody.velocity = new Vector3(v.x, -v.y, v.z);
        }

        if (_rigidBody.transform.position.z > _localBoundsUpperZ)
        {
            _rigidBody.transform.position = new Vector3(p.x, p.y, _localBoundsUpperZ);
            _rigidBody.velocity = new Vector3(v.x, v.y, -v.z);
        }
        else if (_rigidBody.transform.position.z < _localBoundsLowerZ)
        {
            _rigidBody.transform.position = new Vector3(p.x, p.y, _localBoundsLowerZ);
            _rigidBody.velocity = new Vector3(v.x, v.y, -v.z);
        }
    }

   

   
}
