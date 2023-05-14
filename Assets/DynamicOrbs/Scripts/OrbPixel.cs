using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrbPixel : MonoBehaviour
{
    [SerializeField] private OrbsGroup _orbsGroup;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _shiftInterval = 0.5f;
    [SerializeField] private float _shiftTime = 0.25f;

    private float _randomOffset;
    private bool _randomized;

    private float _shiftTimer = 0f;

    private void Start()
    {
        Disable();
        TouchManager.TouchStarted += HandleTouchStarted;
        TouchManager.MultiTouchHappened += HandleMultiTouchHappened;
        TouchManager.TouchEnded += HandleTouchEnded;
        OrbsGroup.InteractionStateChanged += HandleInteractionChange;
    }

    private void HandleInteractionChange(OrbsGroup.OrbInteractionState state)
    {
        if (state == OrbsGroup.OrbInteractionState.Create)
            Disable();
    }

    private void Init()
    {
        _renderer.enabled = true;
    }
    
    private void Disable()
    {
        _renderer.enabled = false;
    }

    private void HandleTouchStarted(Touch touch, TouchManager.TouchZone zone)
    {
        if (OrbsGroup.InteractionState == OrbsGroup.OrbInteractionState.Create) return;
        if (zone != TouchManager.TouchZone.World) return;
        
        
        Init();
    }

    private void HandleTouchEnded(Touch touch, TouchManager.TouchZone zone)
    {
        Disable();
    }
    
    private void HandleMultiTouchHappened(Touch touch, TouchManager.TouchZone zone)
    {
        if (OrbsGroup.InteractionState == OrbsGroup.OrbInteractionState.Create) return;
        if (zone != TouchManager.TouchZone.World) return;
        
        Strobe();
    }

    private void Strobe()
    {
        var _strobeSpeed = 15f;
        var amount = Mathf.Sin(Time.unscaledTime * _strobeSpeed);
        var threshold = 0.5f;
        
        if (Mathf.Abs(amount) > threshold)
            Init();
        else
            Disable();
    }

    private void Update()
    {
        if (OrbsGroup.InteractionState == OrbsGroup.OrbInteractionState.Create) return;

        _shiftTimer += Time.deltaTime;
        if (_shiftTimer >= _shiftInterval)
        {
            StartShift();
            _shiftTimer = 0f;
        }
    }
    
    [Button]
    private void StartShift()
    {
        if (transform.parent)
            transform.parent.DetachChildren();

        if (_orbsGroup.EnabledOrbs.Count <= 0) return;
        
        var index = Random.Range(0, _orbsGroup.EnabledOrbs.Count - 1);
        var newTarget = _orbsGroup.Objects[index];
        StartCoroutine(Shift(newTarget));
    }

    private IEnumerator Shift(Orb target)
    {
        var fromPos = transform.position;
        var fromScale = transform.localScale;
        
        for (float t = 0; t < _shiftTime; t += Time.deltaTime)
        {
            var pos = Vector3.Lerp(fromPos, target.transform.position, t / _shiftTime);
            var scale = Vector3.Lerp(fromScale, target.transform.localScale / 2f, t / _shiftTime);

            transform.position = pos;
            transform.localScale = scale;
            
            yield return null;
        }

        transform.position = target.transform.position;
        transform.localScale = target.transform.localScale / 2f;
        transform.parent = target.transform;
    }

    private void OnDisable()
    {
        EndProcesses();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            EndProcesses();
    }

    private void EndProcesses()
    {
        TouchManager.TouchStarted -= HandleTouchStarted;
        TouchManager.MultiTouchHappened -= HandleMultiTouchHappened;
        TouchManager.TouchEnded -= HandleTouchEnded;
        OrbsGroup.InteractionStateChanged -= HandleInteractionChange;
    }
}
