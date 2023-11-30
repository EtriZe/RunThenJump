public class CharacterStateFactory
{
    CharacterStateMachine _context;

    public CharacterStateFactory(CharacterStateMachine currentContext)
    {
        _context = currentContext;
    }

    public CharacterBaseState Idle(){
        return new CharacterIdleState(_context, this);
    }
    public CharacterBaseState Walk(){
        return new CharacterWalkState(_context, this);
    }
    public CharacterBaseState Run(){
        return new CharacterRunState(_context, this);
    }
    public CharacterBaseState Jump(){
        return new CharacterJumpState(_context, this);
    }
    public CharacterBaseState Grounded(){
        return new CharacterGroundedState(_context, this);
    }
}
