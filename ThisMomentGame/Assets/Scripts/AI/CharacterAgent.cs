using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CharacterAgent : MonoBehaviour
{
    [SerializeField, Tooltip("The radius in which this agent will check its surroundings for other agents")]
    public float seekRadius = 50f;

    [SerializeField]
    public float wanderRadius = 50f;

    [SerializeField]
    public float wanderTimeoutSeconds = 3f;

    public NavMeshAgent navAgentComponent { get; private set; }

    private CharacterStateMachine stateMachine;
    private CS_DummyState dummyState;
    private CS_SeekState seekState;
    private CS_GoToFurthestColorless goToFurthestColorlessState;
    private CS_WanderState wanderState;

    [SerializeField]
    private GameObject testTarget;

    public bool hasColorAssigned { get; private set; } = false;

    public Vector2 aiMoveInput { get; private set; }
    private Vector3 wanderTarget;

    public UnityEvent targetReached = new();

    // Start is called before the first frame update
    void Start()
    {
        dummyState = new CS_DummyState(this.gameObject);
        seekState = new CS_SeekState(this.gameObject);
        goToFurthestColorlessState = new CS_GoToFurthestColorless(this.gameObject);
        wanderState = new CS_WanderState(this.gameObject);
        
        stateMachine = new CharacterStateMachine(dummyState);

        seekState.seekRadius = seekRadius;
        seekState.onFoundAgentWithColor.AddListener(OnSeekFoundAgentWithColor);
        seekState.onFoundAgentWithoutColor.AddListener(OnSeekFoundAgentWithoutColor);
        seekState.onFoundNothing.AddListener(OnSeekFoundNothing);

        goToFurthestColorlessState.onFoundFurtherstColorlessAgent.AddListener(OnGoToFurthestFoundColorlessAgent);
        goToFurthestColorlessState.onReachedFurthestColorlessAgent.AddListener(OnGoToFurthestReachedColorlessAgent);

        wanderState.wanderRadius = wanderRadius;
        wanderState.wanderTimeout = wanderTimeoutSeconds;
        wanderState.onWanderTargetPicked.AddListener(OnWanderStateTargetPicked);
        wanderState.onWanderingComplete.AddListener(OnWanderingComplete);

        //navAgentComponent = GetComponent<NavMeshAgent>();
        navAgentComponent = GetComponentInChildren<NavMeshAgent>();
        if (!navAgentComponent)
        {
            Debug.LogError("Cannot find nav mesh agent component on character agent");
            return;
        }

        //Necessary for 2D
        navAgentComponent.updateUpAxis = false;
        navAgentComponent.updateRotation = false;

        //navAgentComponent.SetDestination(new Vector3(5f, 5f, transform.position.z));
    }

    void Update()
    {
        stateMachine.DoUpdate(Time.deltaTime);

        //If reached wander destination abort
        if ((wanderTarget - transform.position).magnitude <= 1f) navAgentComponent.ResetPath();
    }

    private void FixedUpdate()
    {
        stateMachine.DoFixedUpdate(Time.fixedDeltaTime);

        navAgentComponent.gameObject.transform.localPosition = Vector2.zero;

        if ((navAgentComponent.destination - transform.position).magnitude <= 1f) aiMoveInput = Vector2.zero;
        else aiMoveInput = navAgentComponent.desiredVelocity.normalized;
    }

    public void AssignColor()
    {
        //TODO: Stub function for when a color gets assigned to this agent
        Debug.Log(gameObject.name + "Color has been assigned");

        hasColorAssigned = true;

        GetComponent<ColorChanger>().AssignNewColor(Color.red);

        //Start seeking
        stateMachine.SwitchStates(seekState);
    }

    private void TryAssignColorToOther(CharacterAgent other)
    {
        //Check if the initial 4 assignments have been performed here!
        bool hasInitialAssignments = GameManager.Instance.colorAssignments >= 4;

        if (hasInitialAssignments)
        {
            other.AssignColor();

            stateMachine.SwitchStates(wanderState);
        }
        else
        {
            StartCoroutine(NoInitialAssignmentsTimeoutCo(other));
        }
    }

    private IEnumerator NoInitialAssignmentsTimeoutCo(CharacterAgent otherAgent)
    {
        yield return new WaitForSeconds(1f);

        TryAssignColorToOther(otherAgent);
    }

    public void OnPlayerIntreact()
    {
        AssignColor();
        GameManager.Instance.IncrementColorAssignments();
    }

    private void OnSeekFoundAgentWithColor(CharacterAgent other)
    {
        Debug.Log(gameObject.name + "Seek state found another agent with color already assigned");
    }

    private void OnSeekFoundAgentWithoutColor(CharacterAgent other)
    {
        Debug.Log(gameObject.name + "Seek state found agent without color assigned");

        //TODO: Perform each other's actions here

        stateMachine.SwitchStates(goToFurthestColorlessState);
    }

    private void OnSeekFoundNothing()
    {
        Debug.Log(gameObject.name + "Seek state did not find any other agents");

        stateMachine.SwitchStates(wanderState);
    }

    private void OnGoToFurthestFoundColorlessAgent(CharacterAgent other)
    {
        navAgentComponent.SetDestination(other.transform.position);
    }

    private void OnGoToFurthestReachedColorlessAgent(CharacterAgent other)
    {
        navAgentComponent.ResetPath();
        navAgentComponent.destination = transform.position;
        TryAssignColorToOther(other);
        targetReached.Invoke();
    }

    private void OnWanderStateTargetPicked(Vector3 target)
    {
        Debug.Log(gameObject.name + "Going to new wandering target" + target.ToString());
        navAgentComponent.SetDestination(target);
        wanderTarget = target;
    }

    private void OnWanderingComplete()
    {
        Debug.Log(gameObject.name + "Wandering complete, returning to seek state");
        stateMachine.SwitchStates(seekState);
    }
}
