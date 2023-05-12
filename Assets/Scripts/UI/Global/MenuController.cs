using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    [SerializeField] private Page _initialPage;
    [SerializeField] private Page _startingOverlay;
    [SerializeField] private GameObject _firstFocusItem;

    private Canvas _rootCanvas;
    private Stack<Page> _pageStack = new Stack<Page>();


    private void Awake()
    {
        _rootCanvas = GetComponent<Canvas>();
    }
    
    void Start()
    {
        if (_firstFocusItem != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstFocusItem);
        }

        if (_initialPage != null)
        {
            PushPage(_initialPage);
        }

        if (_startingOverlay != null)
        {
            PushPage(_startingOverlay);
        }
    }

    public void TogglePage(Page page)
    {
        if (_pageStack.Count > 0)
        {
            if (page == _pageStack.Peek())
            {
                PopPage();
                return;
            }
        }
        
        PushPage(page);
    }

    public void PushPage(Page page)
    {
        page.Enter(true);

        if (_pageStack.Count > 0)
        {
            Page currentPage = _pageStack.Peek();

            if (currentPage.exitOnNewPagePush)
            {
                currentPage.Exit(false);
            }
        }
        
        _pageStack.Push(page);
    }

    public void PopPage()
    {
        if (_pageStack.Count > 1)
        {
            Page page = _pageStack.Pop();
            page.Exit(true);

            Page newCurrentPage = _pageStack.Peek();

            // added logic to account for overlays which we do not want to return to
            // only works for a set of two overlays opened in sequence
            if (newCurrentPage.isOverlay)
            {
                _pageStack.Pop();
                return;
            }
            
            if (newCurrentPage.exitOnNewPagePush)
            {
                newCurrentPage.Enter(false);
            }
        }
        else
        {
            Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack");
        }
    }

    private void OnCancel()
    {
        if (_rootCanvas.enabled && _rootCanvas.gameObject.activeInHierarchy)
        {
            if (_pageStack.Count != 0)
            {
                PopPage();
            }
        }
    }

    public void PopAllPages()
    {
        for (int i = 1; i < _pageStack.Count; i++)
        {
            PopPage();
        }
    }

    public bool IsPageInStack(Page page)
    {
        return _pageStack.Contains(page);
    }

    public bool IsPageOnTopOfStack(Page page)
    {
        return _pageStack.Count > 0 && page == _pageStack.Peek();
    }

    public bool IsOnMainPage()
    {
        if (_pageStack.Peek() == _initialPage || _pageStack.Peek() == _startingOverlay)
            return true;
        else
            return false;
    }
}
