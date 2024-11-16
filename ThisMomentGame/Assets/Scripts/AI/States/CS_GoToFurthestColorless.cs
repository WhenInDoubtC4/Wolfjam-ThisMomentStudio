using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CS_GoToFurthestColorless : CharacterState
{
    public CharacterAgent target { get; private set; }

    public UnityEvent<CharacterAgent> onFoundFurtherstColorlessAgent = new();
    public UnityEvent<CharacterAgent> onReachedFurthestColorlessAgent = new();

    private float acceptRadius = 1f;

    bool doReachedCheck = false;
    float checkReachedStartTime = 0f;

    public CS_GoToFurthestColorless(GameObject parent) : base(parent)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Entering CS_GoToFurthestColorless");

        //Find furthest colorless agent
        float dist = -1f;
        CharacterAgent target = null;
        foreach (CharacterAgent agent in GameObject.FindObjectsByType<CharacterAgent>(FindObjectsSortMode.None))
        {
            if (agent.hasColorAssigned) continue;

            float currentDist = (agent.transform.position - parent.transform.position).sqrMagnitude;
            if (currentDist > dist)
            {
                dist = currentDist;
                target = agent;
            }
        }

        if (!target)
        {
            Debug.LogError("Could not find any colorless agents anywhere");
        }
        else
        {
            onFoundFurtherstColorlessAgent.Invoke(target);

            checkReachedStartTime = Time.time;
            doReachedCheck = true;
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log("Exiting CS_GoToFurthestColorless");
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);

        if (!doReachedCheck) return;

        if (Time.time - checkReachedStartTime > 0.5f)
        {
            checkReachedStartTime = Time.time;
        }

        float dist = (target.transform.position - parent.transform.position).magnitude;

        if (dist <= acceptRadius)
        {
            onReachedFurthestColorlessAgent.Invoke(target);
            doReachedCheck = false;
        }
    }
}
