using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CharacterContext
{
    private State _state = null;
    public Character player;
    public CharacterContext(State state, Character character) 
    {
        changeStateTo(state);
        this.player = character;
    }

    public void changeStateTo(State state)
    {
        Debug.Log("Changement d'état vers :" + state.GetType().Name);
        this._state = state;
        this._state.setContext(this);
    }

    public void IdleRequest()
    {
        this._state.Idle();
    }
    public void WalkRequest()
    {
        this._state.Walk();
    }
    public void RunRequest()
    {
        this._state.Run();
    }
    public void JumpRequest()
    {
        this._state.Jump();
    }
    public void CrouchRequest()
    {
        this._state.Crouch();
    }
}
