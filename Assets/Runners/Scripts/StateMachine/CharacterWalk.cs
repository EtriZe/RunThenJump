using UnityEngine;

class CharacterWalk : State
{
    public override void Idle()
    {
        Debug.Log("WALK TO IDLE");
        this._context.changeStateTo(new CharacterIdle());
    }
    public override void Walk()
    {
        Debug.Log("WALKING");
    }
    public override void Run()
    {
        Debug.Log("WALK TO RUN");
        this._context.changeStateTo(new CharacterRun());
    }
    public override void Jump()
    {
        Debug.Log("WALK TO JUMP");
        this._context.changeStateTo(new CharacterJump());
    }
    public override void Crouch()
    {
        Debug.Log("WALK TO CROUCH");
        this._context.changeStateTo(new CharacterCrouch());
    }
}
