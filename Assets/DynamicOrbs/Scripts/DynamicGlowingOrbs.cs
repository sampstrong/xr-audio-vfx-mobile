using System.Collections.Generic;
using System.ComponentModel.Design;
using Niantic.ARDK.Utilities.Input.Legacy;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

// [ExecuteInEditMode]
public class DynamicGlowingOrbs : MonoBehaviour
{
    public List<Orb> Objects => _objects;
    public List<Color> Colors => _colors;
    
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private int _currentBand;
    [SerializeField] private Material _material;
    [SerializeField] private List<Orb> _objects = new List<Orb>();
    [SerializeField] private float _scaleFactor = 0.35f;
    [SerializeField] private LaunchEffect _launchEffect;

    [SerializeField] private Light _light;
    
    [Header("Colors")] 
    [ColorUsageAttribute(true, true)] 
    [SerializeField] private List<Color> _colors = new List<Color>();

    private List<SphereCollider> _colliders = new List<SphereCollider>();
    private List<Vector4> _positions = new List<Vector4>();
    private List<float> _sizes = new List<float>();
    private List<Matrix4x4> _rotationMatrices = new List<Matrix4x4>();

    private List<Orb> _enabledOrbs = new List<Orb>();
    private List<Orb> _disabledOrbs = new List<Orb>();
    private List<Color> _frequencyColors = new List<Color>();

    private Vector3 _origin;

    void Start()
    {
        InitLists();
        ResetOrbs();
        _material.SetInt("_NumberOfObjects", _objects.Count);
    }

    void Update()
    {
        UpdateShaderUniforms();
        CheckForTouch();
    }

    private void CheckForTouch()
    {
        if (PlatformAgnosticInput.touchCount <= 0) return;
        var touch = PlatformAgnosticInput.GetTouch(0);

        if (TouchManager.CurrentTouchZone != TouchManager.TouchZone.World) return;
        
        var touchPos = touch.position;
        var launchEffectPos = _mainCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 0.25f));
        _launchEffect.SetTargetPos(launchEffectPos);
        
        if (touch.phase == TouchPhase.Began)
        {
            _launchEffect.Init(launchEffectPos);
            var launchDistance = 3f;
            var magnitude = 0.5f; // update this to be proportional to the hold length on release
            var position = _mainCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, launchDistance));
            var velocity = _mainCamera.transform.forward * magnitude;
            LaunchOrb(_currentBand, position, velocity);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            _launchEffect.Disable();
        }
    }
    
    

    private void InitLists()
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            var collider = _objects[i].GetComponent<SphereCollider>();
            collider.radius = _scaleFactor * 1.1f; // maybe need to fine tune this value
            _colliders.Add(collider);
            
            var pos3 = _objects[i].transform.position;
            _positions.Add(new Vector4(pos3.x, pos3.y, pos3.z));

            var size = _objects[i].transform.lossyScale.magnitude;
            _sizes.Add(size * _scaleFactor);
            
            var rot = _objects[i].transform.rotation;
            _rotationMatrices.Add(Matrix4x4.Rotate(rot));
        }
    }

    /// <summary>
    /// Launch the next available orb
    /// </summary>
    [Button]
    private void LaunchOrb(int band, Vector3 pos, Vector3 velocity)
    {
        var freq = new OrbFrequency(band, _colors[band]);
        Orb orb;
        
        if (_disabledOrbs.Count > 0)
        {
            // set origin of first orb
            // if (_disabledOrbs.Count == _objects.Count)
            //     _origin = pos + _mainCamera.transform.forward * 4f;
            // take from list of disabled orbs if there are any
            orb = _disabledOrbs[0];
            _disabledOrbs.Remove(orb);
        }
        else
        {
            // if not take the first enabled orb
            orb = _enabledOrbs[0];
            _enabledOrbs.Remove(orb);
        }
        
        // add orb from above to end of enabled orb list an init
        _enabledOrbs.Add(orb);

        var origin = pos + Vector3.up * 1.5f;
        orb.InitOrb(origin, pos, velocity, freq);
    }

    /// <summary>
    /// Disable all orbs and make them ready to launch
    /// </summary>
    [Button]
    private void ResetOrbs()
    {
        _frequencyColors.AddRange(_colors);
        _disabledOrbs.AddRange(_objects);
        _enabledOrbs.Clear();

        foreach (var orb in _objects)
        {
            orb.DisableOrb();
        }
    }

    /// <summary>
    /// Passes each orbs transform and color into the shader
    /// Raymarched objects in shader will then track with objects in unity
    /// </summary>
    private void UpdateShaderUniforms()
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            var pos3 = _objects[i].transform.position;
            _positions[i] = new Vector4(pos3.x, pos3.y, pos3.z, 1.0f);

            var scale = _objects[i].transform.localScale;
            _sizes[i] = scale.x * _scaleFactor;

            var rot = _objects[i].transform.rotation;
            _rotationMatrices[i] = Matrix4x4.Rotate(rot);

            var color = _objects[i].OrbFrequency.color;
            _frequencyColors[i] = color * _objects[i].Intensity;
        }
        
        var posistionsArray = _positions.ToArray();
        var sizesArray = _sizes.ToArray();
        var rotationsArray = _rotationMatrices.ToArray();
        var colorsArray = _frequencyColors.ToArray();

        _material.SetVectorArray("_Positions", posistionsArray);
        _material.SetFloatArray("_Sizes", sizesArray);
        _material.SetMatrixArray("_Rotations", rotationsArray);
        _material.SetColorArray("_Colors", colorsArray);
        _material.SetInt("_NumberOfObjects", _objects.Count);
        
        
        // commented to reduce the amount of data passed to GPU each frame
        // handled internally within shader
        
        // manual override of lighting uniforms to fix bug on iOS build
        // Vector3 lightVector = _light.transform.rotation * Vector3.forward;
        // _material.SetVector("_LightPos", -lightVector);
        // _material.SetColor("_LightCol", _light.color);
        // _material.SetFloat("_GyroidThickness", (Mathf.Sin(Time.unscaledTime * 0.5f) * 0.5f + 0.5f) * 0.1f);
    }

    public void SetCurrentBand(int newBand)
    {
        _currentBand = newBand;
    }
}
