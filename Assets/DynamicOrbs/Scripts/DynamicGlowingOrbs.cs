using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

// [ExecuteInEditMode]
public class DynamicGlowingOrbs : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private List<Orb> _objects = new List<Orb>();
    [SerializeField] private float _scaleFactor = 0.35f;
    [SerializeField] [Range(0, 1)] private float _glowIntensity = 1.0f;
    
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
        _material.SetFloat("_Intensity", _glowIntensity);
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
    private void LaunchOrb(int band)
    {
        var freq = new OrbFrequency(band, _colors[band]);
        
        if (_disabledOrbs.Count > 0)
        {
            var orb = _disabledOrbs[0];
            _disabledOrbs.Remove(orb);
            _enabledOrbs.Add(orb);
            orb.InitOrb(HelperMethods.GetRandomVec3(), HelperMethods.GetRandomVec3(), freq);
        }
        else
        {
            // add a pop in / pop out animation here
            var orb = _enabledOrbs[0];
            _enabledOrbs.Remove(orb);
            _enabledOrbs.Add(orb);
            orb.InitOrb(HelperMethods.GetRandomVec3(), HelperMethods.GetRandomVec3(), freq);
        }
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
            _frequencyColors[i] = color;
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
