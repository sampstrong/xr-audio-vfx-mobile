using System.Collections;
using UnityEngine;

public class TabSlider : MonoBehaviour
{
    [SerializeField] private float _transitionTime = 0.5f;
    [SerializeField] private RectTransform _rectTransform;
    
    public void ChangeTabs(RectTransform newTab)
    {
        var currentPos = _rectTransform.position;
        var newXPos = newTab.position.x;
        var newPos = new Vector3(newXPos, currentPos.y, currentPos.z);

        StartCoroutine(EaseOutQuad(currentPos, newPos));
    }
    
    private IEnumerator Linear(Vector3 currentPos, Vector3 newPos)
    {
        for (float t = 0.0f; t < _transitionTime; t += Time.deltaTime)
        {
            var pos = Vector3.Lerp(currentPos, newPos, t / _transitionTime);
            _rectTransform.position = pos;
            
            yield return null;
        }

        _rectTransform.position = newPos;
    }
    
    private IEnumerator EaseOutQuad(Vector3 currentPos, Vector3 newPos)
    {
        float t = 0;
        
        while (t < _transitionTime)
        {
            t += Time.deltaTime;

            var b = 0; // start value
            var c = 1; // change in value
            var d = _transitionTime; // duration

            t /= d;
            var ease = -c * t * (t - 2) + b;

            var pos = Vector3.Lerp(currentPos, newPos, ease);
            _rectTransform.position = pos;
            
            yield return null;
        }

        _rectTransform.position = newPos;
    }
}
