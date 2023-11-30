using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGroundedState : CharacterBaseState
{
    public CharacterGroundedState(CharacterStateMachine currentContext, CharacterStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {}
    public override void EnterState(){
       // _ctx.Rb.drag = _ctx.GroundDrag;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState(){
       // _ctx.Rb.drag = 0;
    }
    public override void CheckSwitchStates(){
        //if player is grounded and jump is pressed, switch to jump state
        if(_ctx.IsJumpPressed && _ctx.ReadyToJump)
        {
            SwitchState(_factory.Jump());
        }
    }
    public override void InitializeSubState(){}

}
