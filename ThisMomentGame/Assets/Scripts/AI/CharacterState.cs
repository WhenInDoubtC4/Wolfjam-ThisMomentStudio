using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
    public CharacterState()
    {

    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }

    public virtual void DoUpdate(float dt) { }

    public virtual void DoFixedUpdate(float dt) { }
}
