public abstract class CharacterBaseState
{
    protected CharacterStateMachine _ctx;
    protected CharacterStateFactory _factory;
    public CharacterBaseState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory)
    {
        _ctx = currentContext;
        _factory = characterStateFactory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    void UpdateStates(){}
    protected void SwitchState(CharacterBaseState newState)
    {
        //Current state exits state
        ExitState();

        //new state enters state
        newState.EnterState();

        //Switch current State of context
        _ctx.CurrentState = newState;


    }
    protected void SetSuperState(){}
    protected void SetSubState(){}

}
