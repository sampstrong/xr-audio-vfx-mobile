using System;

[Serializable]
public class OrbStateMachine : BaseStateMachine
{
    public CreateState createState;
    public PlayIdleState PlayIdleState;

    public OrbStateMachine()
    {
        this.createState = new CreateState();
        this.PlayIdleState = new PlayIdleState();
    }
}
