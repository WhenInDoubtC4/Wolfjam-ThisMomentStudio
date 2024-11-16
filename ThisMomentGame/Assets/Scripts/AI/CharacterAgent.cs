using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAgent : MonoBehaviour
{
    private NavMeshAgent agentComponent;

    private CharacterStateMachine stateMachine;
    private CS_DummyState dummyState;

    [SerializeField]
    private GameObject testTarget;

    // Start is called before the first frame update
    void Start()
    {
        dummyState = new CS_DummyState();
        stateMachine   = new CharacterStateMachine(dummyState);

        agentComponent = GetComponent<NavMeshAgent>();
        if (!agentComponent)
        {
            Debug.LogError("Cannot find nav mesh agent component on character agent");
            return;
        }

        //Necessary for 2D
        agentComponent.updateUpAxis = false;
        agentComponent.updateRotation = false;

        agentComponent.SetDestination(testTarget.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
