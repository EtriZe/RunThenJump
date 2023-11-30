abstract class State
{
    protected CharacterContext _context;

    public void setContext(CharacterContext character)
    {
        this._context = character;
    }
    public abstract void Idle();
    public abstract void Walk();
    public abstract void Run();
    public abstract void Jump();
    public abstract void Crouch();
}
