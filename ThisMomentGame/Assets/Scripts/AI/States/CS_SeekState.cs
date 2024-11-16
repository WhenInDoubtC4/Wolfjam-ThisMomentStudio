using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CS_SeekState : CharacterState
{
    public float seekRadius { get; set; } = 0f;

    public UnityEvent<CharacterAgent> onFoundAgentWithColor = new();
    public UnityEvent<CharacterAgent> onFoundAgentWithoutColor = new();
    public UnityEvent onFoundNothing = new();

    public CS_SeekState(GameObject parent) : base(parent)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Entering CS_SeekState");

        if (seekRadius <= 0f)
        {
            Debug.LogError("Seek radius is <= 0 in CS_SeekState, this will not work!");
        }
        else
        {
            StartSeeking();
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log("Exiting CS_SeekState");
    }

    private void StartSeeking()
    {
        foreach (CharacterAgent agent in GameObject.FindObjectsByType<CharacterAgent>(FindObjectsSortMode.None))
        {
            //Check if agent is within radius
            if ((agent.transform.position - parent.transform.position).magnitude > seekRadius) continue;

            //Check if agent has color assigned
            if (agent.hasColorAssigned)
            {
                onFoundAgentWithColor.Invoke(agent);
            }
            else
            {
                onFoundAgentWithoutColor.Invoke(agent);
            }
        }

        //Found nothing within the seek radius
        onFoundNothing.Invoke();
    }
}
