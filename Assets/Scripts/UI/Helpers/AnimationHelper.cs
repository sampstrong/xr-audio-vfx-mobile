using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationHelper
{
    public static IEnumerator ZoomIn(RectTransform rTransform, float speed, UnityEvent onEnd)
    {
        float time = 0;
        while (time < 1)
        {
            rTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        rTransform.localScale = Vector3.one;

        onEnd?.Invoke();
    }

    public static IEnumerator ZoomOut(RectTransform rTansform, float speed, UnityEvent onEnd)
    {
        float time = 0;
        while (time < 1)
        {
            rTansform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        rTansform.localScale = Vector3.zero;
        onEnd?.Invoke();
    }

    public static IEnumerator FadeIn(CanvasGroup canvasGroup, float speed, UnityEvent onEnd)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float time = 0;
        while (time < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        canvasGroup.alpha = 1;
        onEnd?.Invoke();
    }

    public static IEnumerator FadeOut(CanvasGroup canvasGroup, float speed, UnityEvent onEnd)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        float time = 0;
        while (time < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        canvasGroup.alpha = 0;
        onEnd?.Invoke();
    }

    public static IEnumerator SlideIn(RectTransform rTransform, Direction direction, float speed, UnityEvent onEnd)
    {
        Vector2 startPosition;
        switch (direction)
        {
            case Direction.Up:
                startPosition = new Vector2(0, -Screen.height);
                break;
            case Direction.Right:
                startPosition = new Vector2(-Screen.width, 0);
                break;
            case Direction.Down:
                startPosition = new Vector2(0, Screen.height);
                break;
            case Direction.Left:
                startPosition = new Vector2(Screen.width, 0);
                break;
            default:
                startPosition = new Vector2(0, -Screen.height);
                break;
        }

        float time = 0;
        while (time < 1)
        {
            rTransform.anchoredPosition = Vector2.Lerp(startPosition, Vector2.zero, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        rTransform.anchoredPosition = Vector2.zero;
        onEnd?.Invoke();
    }

    public static IEnumerator SlideOut(RectTransform rTransform, Direction direction, float speed, UnityEvent onEnd)
    {
        Vector2 endPosition;
        switch (direction)
        {
            case Direction.Up:
                endPosition = new Vector2(0, Screen.height);
                break;
            case Direction.Right:
                endPosition = new Vector2(Screen.width, 0);
                break;
            case Direction.Down:
                endPosition = new Vector2(0, -Screen.height);
                break;
            case Direction.Left:
                endPosition = new Vector2(-Screen.width, 0);
                break;
            default:
                endPosition = new Vector2(0, Screen.height);
                break;
        }

        float time = 0;
        while (time < 1)
        {
            rTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        rTransform.anchoredPosition = endPosition;
        onEnd?.Invoke();
    }
}
