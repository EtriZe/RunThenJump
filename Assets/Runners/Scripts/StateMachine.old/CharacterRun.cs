using UnityEngine;
class CharacterRun : State
{
    public override void Idle()
    {
        Debug.Log("RUN TO IDLE");
        this._context.changeStateTo(new CharacterIdle());
    }
    public override void Walk()
    {
        Debug.Log("RUN TO WALK");
        this._context.changeStateTo(new CharacterWalk());
    }
    public override void Run()
    {
        Debug.Log("RUNNING");
    }
    public override void Jump()
    {
        Debug.Log("RUN TO JUMP");
        this._context.changeStateTo(new CharacterJump());
    }
    public override void Crouch()
    {
        Debug.Log("RUN TO CRUNCH");
        this._context.changeStateTo(new CharacterCrouch());
    }
}
