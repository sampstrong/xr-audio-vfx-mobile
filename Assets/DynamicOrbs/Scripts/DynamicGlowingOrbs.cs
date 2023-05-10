using System.Collections.Generic;
using System.ComponentModel.Design;
using Niantic.ARDK.Utilities.Input.Legacy;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

// [ExecuteInEditMode]
public class DynamicGlowingOrbs : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private int _currentBand;
    [SerializeField] private Material _material;
    [SerializeField] private List<Orb> _objects = new List<Orb>();
    [SerializeField] private float _scaleFactor = 0.35f;
    //[SerializeField] [Range(0, 1)] private float _glowIntensity = 1.0f;
    
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



    void Start()
    {
        InitLists();
        _material.SetInt("_NumberOfObjects", _objects.Count);
    }

    void Update()
    {
        UpdateShaderUniforms();
        CheckForTouch();
        //_material.SetFloat("_Intensity", _glowIntensity);
    }

    private void CheckForTouch()
    {
        if (PlatformAgnosticInput.touchCount < 0) return;
        var touch = PlatformAgnosticInput.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            var launchDistance = 1f;
            var touchPos = touch.position;
            var position = _mainCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, launchDistance));
            Debug.Log($"Touch Pos: {touch.position}, World Pos: {position}");
            // var position = _mainCamera.transform.position + _mainCamera.transform.forward * 2f;
            var magnitude = 1; // update this to be proportional to the hold length on release
            var velocity = _mainCamera.transform.forward * magnitude;
            LaunchOrb(_currentBand, position, velocity);
        }
    }

    private void InitLists()
    {
        ResetOrbs();
        
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
        orb.InitOrb(pos, velocity, freq);
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
    }
}
