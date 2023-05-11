using System;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public static TouchZone CurrentTouchZone => _touchZone;
    public static bool IsTouching => _isTouching;
    public static Vector2 CurrentTouchPos => _touchPos;
    
    public enum TouchZone
    {
        None = 0,
        Bottom = 1,
        Spectrum = 2,
        World = 3,
        Top = 4
    }
    private static TouchZone _touchZone = TouchZone.None;
    

    [SerializeField] private bool _spectrum = true;
    [SerializeField] private TouchZone _guiZone;

    [SerializeField] private float _topDivisor = 7.0f;
    [SerializeField] private float _worldDivisor  = 1.8f;
    [SerializeField] private float _spectrumDivisor = 6.0f;
    [SerializeField] private float _bottomDivisor = 8.0f;

    private static Vector2 _touchPos;
    private static bool _isTouching;

    private int _topSize;
    private int _worldSizeWithSpectrum;
    private int _worldSizeWithoutSpectrum;
    private int _spectrumSize;
    private int _bottomSize;

    private int _topThreshold;
    private int _worldThresholdWithSpectrum;
    private int _worldThresholdWithoutSpectrum;
    private int _spectrumThreshold;
    private int _bottomThreshold;

    public event Action<TouchZone> TouchHappened; 


    private void Update()
    {
        SetTouchZones();

        if (PlatformAgnosticInput.touchCount <= 0)
        {
            _isTouching = false;
            return;
        }
        
        var touch0 = PlatformAgnosticInput.GetTouch(0);
        _isTouching = true;
        _touchPos = touch0.position;

        switch (touch0.position.y)
        {
            case var condition when touch0.position.y > Screen.height - _topThreshold:
                _touchZone = TouchZone.Top;
                // Debug.Log(TouchZone.Top);
                break;
            case var condition when touch0.position.y > Screen.height - _worldThresholdWithSpectrum && 
                                         touch0.position.y < Screen.height - _topThreshold:
                _touchZone = TouchZone.World;
                // Debug.Log(TouchZone.World);
                break;
            case var condition when touch0.position.y > Screen.height - _spectrumThreshold && 
                                         touch0.position.y < Screen.height - _worldThresholdWithSpectrum:
                _touchZone = TouchZone.Spectrum;
                // Debug.Log(TouchZone.Spectrum);
                break;
            case var condition when touch0.position.y > Screen.height - _bottomThreshold && 
                                         touch0.position.y < Screen.height - _spectrumThreshold:
                _touchZone = TouchZone.Bottom;
                // Debug.Log(TouchZone.Bottom);
                break;
            default:
                _touchZone = TouchZone.None;
                // Debug.Log(TouchZone.None);
                break;
        }
    }
    
    public static TouchZone GetCurrentTouchZone()
    {
        return _touchZone;
    }

    private void SetTouchZones()
    {
        // sizes
        _topSize = Screen.height / Mathf.RoundToInt(_topDivisor);
        _worldSizeWithSpectrum = Mathf.RoundToInt(Screen.height / _worldDivisor);
        _worldSizeWithoutSpectrum = Mathf.RoundToInt(Screen.height / 1.35f);
        _spectrumSize = Mathf.RoundToInt(Screen.height / _spectrumDivisor);
        _bottomSize = Screen.height / Mathf.RoundToInt(_bottomDivisor);

        // screen thresholds
        _topThreshold = _topSize;
        _worldThresholdWithSpectrum = _topSize + _worldSizeWithSpectrum;
        _worldSizeWithoutSpectrum = _topSize + _worldSizeWithoutSpectrum;
        _spectrumThreshold = _topSize + _worldSizeWithSpectrum + _spectrumSize;
        _bottomThreshold = _topSize + _worldSizeWithSpectrum + _spectrumSize + _bottomSize;
    }

    private void OnGUI()
    {
        switch (_guiZone)
        {
            case TouchZone.Top:
                GUI.Box(new Rect(0,0, Screen.width, _topSize), "Top");
                break;
            case TouchZone.World:
                if (_spectrum)
                    GUI.Box(new Rect(0, _topSize, Screen.width, _worldSizeWithSpectrum), "World w Spectrum");
                else
                    GUI.Box(new Rect(0, _topSize, Screen.width, _worldSizeWithoutSpectrum), "World w/o Spectrum");
                break;
            case TouchZone.Spectrum:
                GUI.Box(new Rect(0, _topSize + _worldSizeWithSpectrum, Screen.width, _spectrumSize), "Spectrum");
                break;
            case TouchZone.Bottom:
                GUI.Box(new Rect(0, _topSize + _worldSizeWithSpectrum + _spectrumSize, Screen.width, _bottomSize), "Bottom");
                break;
            default:
                break;
        }
    }
}
