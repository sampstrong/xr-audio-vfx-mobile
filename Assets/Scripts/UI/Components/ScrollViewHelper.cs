using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScrollViewHelper : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private VisualElement _firstObject;

    public void ResetScroll()
    {
        StartCoroutine(AnimateToStart());
    }

    private IEnumerator AnimateToStart()
    {
        float duration = 0.25f;
        var startPos = _scrollRect.normalizedPosition;
        var endPos = new Vector2(0, 0);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            var pos = Vector2.Lerp(startPos, endPos, t / duration);
            _scrollRect.normalizedPosition = pos;
            
            yield return null;
        }

        _scrollRect.normalizedPosition = endPos;
    }
}
