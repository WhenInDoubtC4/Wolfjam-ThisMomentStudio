using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dummy state for when the character has no color assigned
public class CS_DummyState : CharacterState
{
    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Entering CS_DummyState");
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log("Exiting CS_DummyState");
    }
}
