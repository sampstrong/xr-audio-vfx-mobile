public interface IState
{
    /// <summary>
    /// Code that runs when we first enter the state
    /// </summary>
    public void Enter();

    /// <summary>
    /// Per-frame logic, include condition to transition to a new state
    /// </summary>
    public void Update();

    /// <summary>
    /// Code that runs when we exit the state
    /// </summary>
    public void Exit();
}
