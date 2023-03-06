using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PixelGlitch : MonoBehaviour
{
    public List<Vector3> CurrentPositions => _currentPositions;
    public List<Vector3> CurrentScales => _currentScales;
    public int NumberOfPixels => _numberOfPixels;

    public event Action<int, Vector3> PositionUpdated;
    public event Action<int, Vector3> ScaleUpdated;
    public event Action PixelsInitialized;

    [SerializeField] private GameObject _pixelPrefab;
    [SerializeField] private int _numberOfPixels;
    [SerializeField] private float _transitionDuration = 0.25f;
    
    [Header("Position")]
    [SerializeField] private Range _xPosRange;
    [SerializeField] private Range _yPosRange;
    [SerializeField] private Range _zPosRange;
    
    [Header("Scale")]
    [SerializeField] private Range _xScaleRange;
    [SerializeField] private Range _yScaleRange;
    [SerializeField] private Range _zScaleRange;
    
    private List<Pixel> _pixels = new List<Pixel>();
    
    private List<Vector3> _newPositions = new List<Vector3>();
    private List<Vector3> _currentPositions = new List<Vector3>();
    private List<Vector3> _oldPositions = new List<Vector3>();
    
    private List<Vector3> _newScales = new List<Vector3>();
    private List<Vector3> _currentScales = new List<Vector3>();
    private List<Vector3> _oldScales = new List<Vector3>();

    private float _transitionTimer = 0.0f;
    private float _transitionTime = 3.0f;

    [Serializable]
    public class Range
    {
        public float min;
        public float max;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        PixelsInitialized += Test;
    }

    private void Test()
    {
        Debug.Log("Test Worked");
    }

    private void Update()
    {
        _transitionTimer += Time.deltaTime;
        if (_transitionTimer >= _transitionTime)
        {
            SetNewRandomPosScale();
            StartCoroutine(Reposition(_oldPositions, _newPositions, _transitionDuration));
            StartCoroutine(Rescale(_oldScales, _newScales, _transitionDuration));
            _transitionTimer = 0.0f;
        }
    }

    private void Init()
    {
        for (int i = 0; i < _numberOfPixels; i++)
        {
            GetRandomPosScale(out Vector3 pos, out Vector3 scale);
            AddToLists(pos, scale);
            
            var newObject = Instantiate(_pixelPrefab, _newPositions[i], Quaternion.identity, this.transform);

            newObject.transform.localScale = _newScales[i];
            var renderer = newObject.GetComponent<Renderer>();

            var newPixel = new Pixel(newObject, renderer);
            _pixels.Add(newPixel);
        }

        PixelsInitialized?.Invoke();
    }
    
    

    private void AddToLists(Vector3 pos, Vector3 scale)
    {
        _oldPositions.Add(pos);
        _currentPositions.Add(pos);
        _newPositions.Add(pos);
        _oldScales.Add(scale);
        _currentScales.Add(scale);
        _newScales.Add(scale);
    }

    private void SetNewRandomPosScale()
    {
        _oldPositions = new List<Vector3>(_newPositions);
        _oldScales = new List<Vector3>(_newScales);
        
        for (int i = 0; i < _numberOfPixels; i++)
        {
            GetRandomPosScale(out Vector3 pos, out Vector3 scale);

            _newPositions[i] = pos;
            _newScales[i] = scale;
        }
    }

    private void GetRandomPosScale(out Vector3 pos, out Vector3 scale)
    {
        var p = GetRandomV3(
            _xPosRange.min, _xPosRange.max, 
            _yPosRange.min, _yPosRange.max, 
            _zPosRange.min, _zPosRange.max);
        var s = GetRandomV3(
            _xScaleRange.min, _xScaleRange.max,
            _yScaleRange.min, _yScaleRange.max,
            _zScaleRange.min, _zScaleRange.max);
        
        var localPos = transform.TransformPoint(p);
        
        pos = localPos;
        scale = s;
    }

    private Vector3 GetRandomV3(
        float xMin, float xMax, 
        float yMin, float yMax, 
        float zMin, float zMax)
    {
        float x = Random.Range(xMin, xMax);
        float y = Random.Range(yMin, yMax);
        float z = Random.Range(zMin, zMax);

        return new Vector3(x, y, z);
    }

    private IEnumerator Reposition(List<Vector3> oldPositions, List<Vector3> newPositions, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            for (int i = 0; i < _pixels.Count; i++)
            {
                var pos = Vector3.Lerp(oldPositions[i], newPositions[i], t / duration);
                _pixels[i].Obj.transform.position = pos;
                UpdateCurrentPosition(i, pos);
            }
            yield return null;
        }

        for (int i = 0; i < _pixels.Count; i++)
        {
            _pixels[i].Obj.transform.position = newPositions[i];
            UpdateCurrentPosition(i, newPositions[i]);
        }
    }
    
    private IEnumerator Rescale(List<Vector3> oldScales, List<Vector3> newScales, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            for (int i = 0; i < _pixels.Count; i++)
            {
                var scale = Vector3.Lerp(oldScales[i], newScales[i], t / duration);
                _pixels[i].Obj.transform.localScale = scale;
                UpdateCurrentScale(i, scale);
            }
            yield return null;
        }

        for (int i = 0; i < _pixels.Count; i++)
        {
            _pixels[i].Obj.transform.localScale = newScales[i];
            UpdateCurrentScale(i, newScales[i]);
        }
    }

    private void UpdateCurrentPosition(int index, Vector3 pos)
    {
        _currentPositions[index] = pos;
        PositionUpdated?.Invoke(index, pos);
    }

    private void UpdateCurrentScale(int index, Vector3 scale)
    {
        _currentScales[index] = scale;
        ScaleUpdated?.Invoke(index, scale);
    }
    
}
