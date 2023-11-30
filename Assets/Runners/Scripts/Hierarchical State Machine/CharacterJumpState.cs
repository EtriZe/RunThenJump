using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CharacterJumpState : CharacterBaseState
{
    public CharacterJumpState(CharacterStateMachine currentContext, CharacterStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    { }
    public override void EnterState()
    {
        HandleJump();
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.IsJumpPressed = false;
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.Grounded)
        {
            SwitchState(_factory.Grounded());
        }
    }
    public override void InitializeSubState() { }

    void HandleJump()
    {
        _ctx.ReadyToJump = false;
        _ctx.Rb.velocity = new Vector3(_ctx.Rb.velocity.x, 0f, _ctx.Rb.velocity.z);
        _ctx.Rb.AddForce(_ctx.Player.transform.up * _ctx.JumpPower, ForceMode.Impulse);
        _ctx.StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.25f);
        _ctx.ReadyToJump = true;
    }
}
