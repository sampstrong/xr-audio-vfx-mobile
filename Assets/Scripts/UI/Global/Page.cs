using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource), typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class Page : MonoBehaviour
{
    public bool exitOnNewPagePush = false;
    public bool isOverlay = false;

    [SerializeField] private float _animationSpeed = 1.0f;
    [SerializeField] private AudioClip _entryClip;
    [SerializeField] private AudioClip _exitClip;
    [SerializeField] private EntryMode _entryMode = EntryMode.Slide;
    [SerializeField] private Direction _entryDirection = Direction.Left;
    [SerializeField] private EntryMode _exitMode = EntryMode.Slide;
    [SerializeField] private Direction _exitDirection = Direction.Left;

    [SerializeField] private UnityEvent _prePushAction;
    [SerializeField] private UnityEvent _postPushAction;
    [SerializeField] private UnityEvent _prePopAction;
    [SerializeField] private UnityEvent _postPopAction;
    

    private AudioSource _audioSource;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private Coroutine _animationCoroutine;
    private Coroutine _audioCoroutine;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 0;
        _audioSource.enabled = false;
    }

    public void Enter(bool playAudio)
    {
        _prePushAction?.Invoke();
        
        switch (_entryMode)
        {
            case EntryMode.Slide:
                SlideIn(playAudio);
                break;
            case EntryMode.Zoom:
                ZoomIn(playAudio);
                break;
            case EntryMode.Fade:
                FadeIn(playAudio);
                break;
        }
    }
    
    public void Exit(bool playAudio)
    {
        _prePopAction?.Invoke();
        
        switch (_exitMode)
        {
            case EntryMode.Slide:
                SlideOut(playAudio);
                break;
            case EntryMode.Zoom:
                ZoomOut(playAudio);
                break;
            case EntryMode.Fade:
                FadeOut(playAudio);
                break;
        }
    }

    private void SlideIn(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine =
            StartCoroutine(AnimationHelper.SlideIn(_rectTransform, _entryDirection, _animationSpeed, _postPushAction));

        PlayEntryClip(playAudio);
    }
    
    private void SlideOut(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine =
            StartCoroutine(AnimationHelper.SlideOut(_rectTransform, _exitDirection, _animationSpeed, _postPopAction));

        PlayExitClip(playAudio);
    }
    
    private void ZoomIn(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine =
            StartCoroutine(AnimationHelper.ZoomIn(_rectTransform, _animationSpeed, _postPushAction));

        PlayEntryClip(playAudio);
    }
    
    private void ZoomOut(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine =
            StartCoroutine(AnimationHelper.ZoomOut(_rectTransform, _animationSpeed, _postPopAction));

        PlayExitClip(playAudio);
    }
    
    private void FadeIn(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine =
            StartCoroutine(AnimationHelper.FadeIn(_canvasGroup, _animationSpeed, _postPushAction));

        PlayEntryClip(playAudio);
    }
    
    private void FadeOut(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine =
            StartCoroutine(AnimationHelper.FadeOut(_canvasGroup, _animationSpeed, _postPopAction));

        PlayExitClip(playAudio);
    }

    private void PlayEntryClip(bool playAudio)
    {
        if (playAudio && _entryClip != null && _audioSource != null)
        {
            if (_audioCoroutine != null)
            {
                StopCoroutine(_audioCoroutine);
            }

            _audioCoroutine = StartCoroutine(PlayClip(_entryClip));
        }
    }
    
    private void PlayExitClip(bool playAudio)
    {
        if (playAudio && _exitClip != null && _audioSource != null)
        {
            if (_audioCoroutine != null)
            {
                StopCoroutine(_audioCoroutine);
            }

            _audioCoroutine = StartCoroutine(PlayClip(_exitClip));
        }
    }
    
    private IEnumerator PlayClip(AudioClip clip)
    {
        _audioSource.enabled = true;

        WaitForSeconds wait = new WaitForSeconds(clip.length);
        
        _audioSource.PlayOneShot(clip);

        yield return wait;

        _audioSource.enabled = false;
    }

}
