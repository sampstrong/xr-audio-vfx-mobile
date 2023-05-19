using System.Collections;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.Extensions;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private ARSessionManager _arSessionManager;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;

    private void Start()
    {
        // _arSessionManager.ARSession.Ran += StartFade;
        _arSessionManager.ARSessionCreated += StartFade;
    }

    private void StartFade()
    {
        StartCoroutine(FadeSplashScreen());
    }

    private IEnumerator FadeSplashScreen()
    {
        yield return new WaitForSeconds(2f);
        
        for (float t = 0; t < _fadeDuration; t += Time.deltaTime)
        {
            var alpha = Mathf.Lerp(1.0f, 0.0f, t / _fadeDuration);
            _canvasGroup.alpha = alpha;
            
            yield return null;
        }

        _canvasGroup.alpha = 0.0f;
    }
}
