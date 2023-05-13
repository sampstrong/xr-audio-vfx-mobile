using System;

[Serializable]
public abstract class BaseStateMachine
{
    public virtual IState CurrentState { get; private set; }

    public virtual event Action<IState> OnTransition;
    public virtual void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    public virtual void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
        OnTransition?.Invoke(nextState);
    }

    public virtual void Update()
    {
        if (CurrentState != null)
            CurrentState.Update();
    }
}
