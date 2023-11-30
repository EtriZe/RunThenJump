using UnityEngine;
class CharacterCrouch : State
{
    public override void Idle()
    {
        Debug.Log("CROUCH TO IDLE");
        this._context.changeStateTo(new CharacterIdle());
    }
    public override void Walk()
    {
        Debug.Log("CROUCH TO WALK");
        this._context.changeStateTo(new CharacterWalk());
    }
    public override void Run()
    {
        Debug.Log("CROUCH TO RUN");
        this._context.changeStateTo(new CharacterRun());
    }
    public override void Jump()
    {
        Debug.Log("CROUCH TO JUMP");
        this._context.changeStateTo(new CharacterJump());
    }
    public override void Crouch()
    {
        Debug.Log("CROUCHING");
    }
}
