using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
    protected GameObject parent { get; private set; }

    public CharacterState(GameObject parent)
    {
        this.parent = parent;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }

    public virtual void DoUpdate(float dt) { }

    public virtual void DoFixedUpdate(float dt) { }
}
