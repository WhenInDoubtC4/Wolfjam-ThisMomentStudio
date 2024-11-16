using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CS_WanderState : CharacterState
{
    public float wanderRadius { get; set; } = 0f;

    public float wanderTimeout { get; set; } = 1f;

    public UnityEvent<Vector3> onWanderTargetPicked = new();
    public UnityEvent onWanderingComplete = new();

    private bool isWandering = false;
    private float wanderStartTime = 0f;

    public CS_WanderState(GameObject parent) : base(parent)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Entering CS_WanderState");

        //Pick a new point within the target radius
        Vector3 target = new(Random.Range(2f, wanderRadius), Random.Range(2f, wanderRadius), 0f);
        target += parent.transform.position;
        onWanderTargetPicked.Invoke(target);

        //Start wandering (timeout)
        wanderStartTime = Time.time;
        isWandering = true;
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log("Exiting CS_WanderState");
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);

        if (!isWandering) return;

        if (Time.time - wanderStartTime < wanderTimeout) return;

        onWanderingComplete.Invoke();
    }
}
