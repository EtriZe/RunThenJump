using UnityEngine;
using System.Collections;

class CharacterJump : State
{
    public override void Idle()
    {
        Debug.Log("JUMP TO IDLE");
        this._context.changeStateTo(new CharacterIdle());
    }
    public override void Walk()
    {
        Debug.Log("JUMP TO WALK");
        this._context.changeStateTo(new CharacterWalk());
    }
    public override void Run()
    {
        Debug.Log("JUMP TO RUN");
        this._context.changeStateTo(new CharacterRun());
    }
    public override void Jump()
    {
        Debug.Log("JUMPING");
    }
    public override void Crouch()
    {
        Debug.Log("JUMP TO CROUCH");
        this._context.changeStateTo(new CharacterCrouch());
    }
}
