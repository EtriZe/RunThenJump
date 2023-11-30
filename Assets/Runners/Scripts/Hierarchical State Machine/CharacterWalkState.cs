using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalkState : CharacterBaseState
{
    public CharacterWalkState(CharacterStateMachine currentContext, CharacterStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    { }
    public override void EnterState(){}
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState(){}
    public override void CheckSwitchStates(){}
    public override void InitializeSubState(){}

}

