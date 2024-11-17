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
    private CS_GoToSetColorless goToSetColorlessState;
    private CS_WanderState wanderState;

    [SerializeField]
    private GameObject testTarget;

    public bool hasColorAssigned { get; private set; } = false;

    public Vector2 aiMoveInput { get; private set; }
    private Vector3 wanderTarget;

    public UnityEvent targetReached = new();

    private Coroutine timeoutCo;

    EmoteEnum currentColor;

    // Start is called before the first frame update
    void Start()
    {
        dummyState = new CS_DummyState(this.gameObject);
        seekState = new CS_SeekState(this.gameObject);
        goToFurthestColorlessState = new CS_GoToFurthestColorless(this.gameObject);
        goToSetColorlessState = new CS_GoToSetColorless(this.gameObject);
        wanderState = new CS_WanderState(this.gameObject);
        
        stateMachine = new CharacterStateMachine(dummyState);

        seekState.seekRadius = seekRadius;
        seekState.onFoundAgentWithColor.AddListener(OnSeekFoundAgentWithColor);
        seekState.onFoundAgentWithoutColor.AddListener(OnSeekFoundAgentWithoutColor);
        seekState.onFoundNothing.AddListener(OnSeekFoundNothing);

        goToFurthestColorlessState.onFoundFurtherstColorlessAgent.AddListener(OnGoToFurthestFoundColorlessAgent);
        goToFurthestColorlessState.onReachedFurthestColorlessAgent.AddListener(OnGoToFurthestReachedColorlessAgent);

        goToSetColorlessState.onTargetReached.AddListener(OnSetColorlessTargetReached);

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

        GetComponent<ConnectionManager>().onConnectionFinished.AddListener(OnPlayerIntreact);
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

        if ((navAgentComponent.destination - transform.position).magnitude <= 0.08f)
        {
            aiMoveInput = Vector2.zero;
            targetReached.Invoke();
        }
        else
        {
            aiMoveInput = navAgentComponent.desiredVelocity.normalized;
        }
    }

    public void AssignColor(EmoteEnum color)
    {
        Debug.Log(color.ToString());
        //TODO: Stub function for when a color gets assigned to this agent
        Debug.Log(gameObject.name + "Color has been assigned");

        hasColorAssigned = true;

        Color col = Color.white;
        switch (color)
        {
            case EmoteEnum.Red:        
                col = new Color(244.0f/255.0f, 99.0f / 255.0f, 81.0f / 255.0f); // orangey red
                break;
            case EmoteEnum.Blue:
                col = new Color(42.0f / 255.0f, 219.0f / 255.0f, 178.0f / 255.0f); // cyany
                break;
            case EmoteEnum.Green:
                col = new Color(233.0f / 255.0f, 86.0f / 255.0f, 186.0f / 255.0f); // magentay
                break;
            case EmoteEnum.Yellow:
                col = new Color(244.0f / 255.0f, 196.0f / 255.0f, 106.0f / 255.0f); // yellowy
                break;
        }

        currentColor = color;

        GetComponent<ColorChanger>().AssignNewColor(col);

        //Start seeking
        stateMachine.SwitchStates(seekState);
    }

    private void TryAssignColorToOther(CharacterAgent other)
    {
        //Check if the initial 4 assignments have been performed here!
        //bool hasInitialAssignments = GameManager.Instance.colorAssignments >= 4;

        //if (hasInitialAssignments)
        //{
            if (timeoutCo != null) StopCoroutine(timeoutCo);
            timeoutCo = null;

            other.AssignColor(currentColor);

            stateMachine.SwitchStates(wanderState);
        //}
        //else
        //{
            //timeoutCo = StartCoroutine(NoInitialAssignmentsTimeoutCo(other));
        //    stateMachine.SwitchStates(wanderState);
        //}
    }

    private IEnumerator NoInitialAssignmentsTimeoutCo(CharacterAgent otherAgent)
    {
        yield return new WaitForSeconds(1f);

        TryAssignColorToOther(otherAgent);
    }

    public void OnPlayerIntreact(EmoteEnum type)
    {
        //Already as a color assigned
        if (hasColorAssigned) return;

        Debug.Log(type.ToString());

        AssignColor(type);
        GameManager.Instance.IncrementColorAssignments();
    }

    private void OnSeekFoundAgentWithColor(CharacterAgent other)
    {
        Debug.Log(gameObject.name + "Seek state found another agent with color already assigned");

        //TODO: Perform each other's actions here

        stateMachine.SwitchStates(goToFurthestColorlessState);
    }

    private void OnSeekFoundAgentWithoutColor(CharacterAgent other)
    {
        Debug.Log(gameObject.name + "Seek state found agent without color assigned");

        //TODO: Walk to target and try assign color
        stateMachine.SwitchStates(goToSetColorlessState);
        goToSetColorlessState.target = other;
        navAgentComponent.SetDestination(new Vector3(other.transform.position.x, other.transform.position.y, transform.position.z));
    }

    private void OnSetColorlessTargetReached(CharacterAgent other)
    {
        navAgentComponent.ResetPath();
        navAgentComponent.destination = transform.position;
        TryAssignColorToOther(other);
    }

    private void OnSeekFoundNothing()
    {
        Debug.Log(gameObject.name + "Seek state did not find any other agents");

        stateMachine.SwitchStates(wanderState);
    }

    private void OnGoToFurthestFoundColorlessAgent(CharacterAgent other)
    {
        navAgentComponent.SetDestination(new Vector3(other.transform.position.x, other.transform.position.y, transform.position.z));
    }

    private void OnGoToFurthestReachedColorlessAgent(CharacterAgent other)
    {
        navAgentComponent.ResetPath();
        navAgentComponent.destination = transform.position;
        TryAssignColorToOther(other);
    }

    private void OnWanderStateTargetPicked(Vector3 target)
    {
        Debug.Log(gameObject.name + "Going to new wandering target" + target.ToString());
        navAgentComponent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
        wanderTarget = target;
    }

    private void OnWanderingComplete()
    {
        Debug.Log(gameObject.name + "Wandering complete, returning to seek state");
        stateMachine.SwitchStates(seekState);
    }
}
