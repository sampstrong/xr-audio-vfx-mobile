using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Selects bands based on a user-facing index from 1 to 8
/// Note this differs from other parts of the codebase where the band index goes from 0 to 7 as typical in code
/// </summary>
public class BandSelector : MonoBehaviour
{
    [SerializeField] private DynamicGlowingOrbs _orbs;
    [SerializeField] private LaunchEffect _launchEffect;
    [SerializeField] private SimpleSpectrum _spectrum;
    [SerializeField] private BandButton[] _bands = new BandButton[8];

    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _deselectedColor;
    
    [ColorUsageAttribute(true, true)] 
    [SerializeField] private Color _currentColor;
    [SerializeField] private TextMeshProUGUI _currentBandText;

    private int _currentBand = 0;
    private int _currentBandLast = 0;

    [Button]
    private void ChangeColor(Color color)
    {
        _spectrum.colorMin = color;
        _spectrum.colorMax = color * 2f;
        _spectrum.RebuildSpectrum();
    }

    private void Update()
    {
        IsPointerOverUIElement();
        UpdateCurrentBand();
    }
    
    

    ///Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    
    
    ///Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults )
    {
        for(int index = 0;  index < eventSystemRaysastResults.Count; index ++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                foreach (var band in _bands)
                {
                    if (curRaysastResult.gameObject == band.gameObject)
                    {
                        band.image.color = _selectedColor;
                        _currentBand = band.userIndex;
                    }
                    else
                        band.image.color = _deselectedColor;
                }
                
                return true;
            }
                
        }
        return false;
    }
    
    
    ///Gets all event systen raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position =  Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventData, raysastResults );
        return raysastResults;
    }

    private void UpdateCurrentBand()
    {
        if (_currentBand != _currentBandLast)
        {
            _currentBandText.text = $"BAND {_currentBand}";
            _currentBandLast = _currentBand;

            var nonUserBandIndex = _currentBand - 1;
            
            ChangeColor(_orbs.Colors[nonUserBandIndex]);
            _orbs.SetCurrentBand(nonUserBandIndex);
            _launchEffect.SetColor(_orbs.Colors[nonUserBandIndex]);
            
        }
    }
}
