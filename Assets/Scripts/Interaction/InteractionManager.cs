using System;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Class handles global interaction state and interactions for debugging
/// </summary>
public class InteractionManager : Singleton<InteractionManager>
{
    public Camera MainCamera { get => _mainCamera; }
    
    public enum InteractionState
    {
        Inactive = 0,
        Active = 1,
        Spawning = 2
    }

    [SerializeField] private InteractionState _interactionState;

    public InteractionState CurrentInteractionState
    {
        get => _interactionState;
        set => SetInteractionState(value); 
    }

    public event Action onInteractionStateChanged;
    
    [SerializeField] private Camera _mainCamera;
    

    private void SetInteractionState(InteractionState state)
    {
        _interactionState = state;
        onInteractionStateChanged?.Invoke();
    }

    public void SetStateByIndex(int index)
    {
        SetInteractionState((InteractionState)index);
    }
    
    
    
}
