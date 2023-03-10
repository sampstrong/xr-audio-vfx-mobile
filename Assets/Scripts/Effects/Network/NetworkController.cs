using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class NetworkController : MonoBehaviour
{
    #region Parameter Access
    public List<Vector3> CurrentPositions => _currentPositions;
    //public List<Pixel> Pixels => _pixels;
    public List<Vector3> CurrentScales => _currentScales;
    public int NetworkSize => _networkSize;
    public AnimationState CurrentAnimationState => _animationState;

    public event Action<int, Vector3> PositionUpdated;
    public event Action<int, Vector3> ScaleUpdated;
    public event Action NetworkInitialized;
    public event Action VerticalOffsetStarted;
    public event Action VerticalOffsetEnded;

    #endregion

    #region Serialized Fields

    
    [Header("Network Parameters")]
    [SerializeField] private int _band = 1;
    [SerializeField] private int _networkSize;
    [SerializeField] private float _transitionDuration = 0.25f;
    
    [Header("Position")]
    [SerializeField] private Range _xPosRange;
    [SerializeField] private Range _yPosRange;
    [SerializeField] private Range _zPosRange;
    
    [Header("Scale")]
    [SerializeField] private Range _xScaleRange;
    [SerializeField] private Range _yScaleRange;
    [SerializeField] private Range _zScaleRange;
    
    [Serializable]
    public class Range
    {
        public float min;
        public float max;
    }

    #endregion
    
    #region Private Variables

    
    private List<Vector3> _newPositions = new List<Vector3>();
    private List<Vector3> _currentPositions = new List<Vector3>();
    private List<Vector3> _oldPositions = new List<Vector3>();
    
    private List<Vector3> _newScales = new List<Vector3>();
    private List<Vector3> _currentScales = new List<Vector3>();
    private List<Vector3> _oldScales = new List<Vector3>();

    #endregion

    #region Enums

    public enum AnimationState
    {
        None = 0,
        PosScale = 1,
        VerticalOffset = 2
    }

    private AnimationState _animationState;

    #endregion
    
    void Start()
    {
        Init();
        VFXEventManager.onHalfBar += FilterHalfBarTrigger;
        VFXEventManager.onBar += FilterBarTrigger;
    }

    private void Init()
    {
        for (int i = 0; i < _networkSize; i++)
        {
            GetRandomPosScale(out Vector3 pos, out Vector3 scale);
            AddToLists(pos, scale);
        }

        NetworkInitialized?.Invoke();

        for (int i = 0; i < _networkSize; i++)
        {
            PositionUpdated?.Invoke(i, _newPositions[i]);
            ScaleUpdated?.Invoke(i, _newScales[i]);
        }

        _animationState = AnimationState.None;
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
        
        for (int i = 0; i < _networkSize; i++)
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

    private void FilterHalfBarTrigger(int band)
    {
        if (band != _band) return;
        if (_animationState == AnimationState.None)
            StartVerticalShift();
    }

    private void FilterBarTrigger(int band)
    {
        if (band != _band) return;
        if (_animationState == AnimationState.None)
            ChangePosScale();
    }

    private void ChangePosScale()
    {
        SetNewRandomPosScale();
        StartCoroutine(Reposition(_oldPositions, _newPositions, _transitionDuration));
        StartCoroutine(Rescale(_oldScales, _newScales, _transitionDuration));
    }
    
    private void StartVerticalShift()
    {
        if (ServiceLocator.Instance.EffectManager.CurrentPreset is EffectManager.Preset.Drop
            or EffectManager.Preset.Base)
        {
            List<Vector3> randYPositions = new List<Vector3>();
            for (int i = 0; i < _networkSize; i++)
            {
                var newPos = GetRandomV3(0, 0, _yPosRange.min, _yPosRange.max, 0, 0);
                var newLocalPos = transform.TransformPoint(newPos);
                
                randYPositions.Add(new Vector3(_currentPositions[i].x, newLocalPos.y, _currentPositions[i].z));
            }

            StartCoroutine(ShiftVertically(_newPositions, randYPositions, _transitionDuration));
        }
    }
    

    #region Animation Coroutines

        private IEnumerator Reposition(List<Vector3> oldPositions, List<Vector3> newPositions, float duration)
        {
            _animationState = AnimationState.PosScale;
            
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                for (int i = 0; i < _networkSize; i++)
                {
                    var pos = Vector3.Lerp(oldPositions[i], newPositions[i], t / duration);
                    UpdateCurrentPosition(i, pos);
                }
                yield return null;
            }
    
            for (int i = 0; i < _networkSize; i++)
            {
                UpdateCurrentPosition(i, newPositions[i]);
            }

            _animationState = AnimationState.None;
        }
    
        private IEnumerator Rescale(List<Vector3> oldScales, List<Vector3> newScales, float duration)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                for (int i = 0; i < _networkSize; i++)
                {
                    var scale = Vector3.Lerp(oldScales[i], newScales[i], t / duration);
                    UpdateCurrentScale(i, scale);
                }
                yield return null;
            }
    
            for (int i = 0; i < _networkSize; i++)
            {
                UpdateCurrentScale(i, newScales[i]);
            }
        }
    
        private IEnumerator ShiftVertically(List<Vector3> oldPositions, List<Vector3> newPositions, float duration)
        {
            _animationState = AnimationState.VerticalOffset;
            VerticalOffsetStarted?.Invoke();
            
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                for (int i = 0; i < _networkSize; i++)
                {
                    var pos = Vector3.Lerp(oldPositions[i], newPositions[i], t / duration);
                    UpdateCurrentPosition(i, pos);
                }
                yield return null;
            }

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                for (int i = 0; i < _networkSize; i++)
                {
                    var pos = Vector3.Lerp(newPositions[i], oldPositions[i], t / duration);
                    UpdateCurrentPosition(i, pos);
                }
                yield return null;
            }
    
            for (int i = 0; i < _networkSize; i++)
            {
                UpdateCurrentPosition(i, oldPositions[i]);
            }

            _animationState = AnimationState.None;
            VerticalOffsetEnded?.Invoke();
        }

    #endregion
    

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

    public void TriggerPosUpdated()
    {
        for (int i = 0; i < _currentPositions.Count; i++)
        {
            PositionUpdated?.Invoke(i, _currentPositions[i]);
        }
        
    }
    
}
