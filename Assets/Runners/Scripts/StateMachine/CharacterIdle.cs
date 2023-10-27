using UnityEngine;
class CharacterIdle : State
{
    public override void Idle()
    {
        Debug.Log("IDLING");
    }
    public override void Walk()
    {
        Debug.Log("IDLE TO WALK");
        this._context.changeStateTo(new CharacterWalk());
    }
    public override void Run()
    {
        Debug.Log("IDLE TO RUN");
        this._context.changeStateTo(new CharacterRun());
    }
    public override void Jump()
    {
        Debug.Log("IDLE TO JUMP");
        this._context.changeStateTo(new CharacterJump());
    }
    public override void Crouch()
    {
        Debug.Log("IDLE TO Crouch");
        this._context.changeStateTo(new CharacterCrouch());
    }
}
