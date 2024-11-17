using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CS_GoToSetColorless : CharacterState
{
    public CharacterAgent target { get; set; }

    public UnityEvent<CharacterAgent> onTargetReached = new();

    private bool keepChecking = false;

    public CS_GoToSetColorless(GameObject parent) : base(parent)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Entering CS_GoToSetColorless");
        keepChecking = true;
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log("Exiting CS_GoToSetColorless");
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);

        if (!target || !keepChecking) return;

        //Keep checking if target has been reached
        if ((target.transform.position - parent.transform.position).magnitude <= 1f)
        {
            onTargetReached.Invoke(target);
            keepChecking = false;
            return;
        }
    }
}
