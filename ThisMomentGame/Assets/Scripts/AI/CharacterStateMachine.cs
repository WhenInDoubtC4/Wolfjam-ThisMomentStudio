using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine
{
    private CharacterState currentState;

    public CharacterStateMachine(CharacterState initialState)
    {
        currentState = initialState;
        currentState.EnterState();
    }

    public void SwitchStates(CharacterState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public void DoUpdate(float dt)
    {
        currentState.DoUpdate(dt);
    }

    public void DoFixedUpdate(float dt)
    {
        currentState.DoFixedUpdate(dt);
    }
}
