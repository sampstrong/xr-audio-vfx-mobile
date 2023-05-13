using UnityEngine;

public class StateManager : MonoBehaviour
{
    private BaseStateMachine _currentStateMachine;

    private void Start()
    {
        // placeholder
        // call this externally in the future when we have multiple state machines and effects
        InitStateMachine(new OrbStateMachine()); 
    }

    public void InitStateMachine(BaseStateMachine stateMachine)
    {
        _currentStateMachine = stateMachine;
        _currentStateMachine.Initialize(new CreateState());
    }

    private void Update()
    {
        _currentStateMachine.Update();
    }
}
