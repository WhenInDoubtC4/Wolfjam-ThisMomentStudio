using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConnectionPoint : MonoBehaviour
{
    public Transform SnapPoint;
    public GameObject characterObject;
    public UnityEvent onInteract= new UnityEvent();

    public float triggerRadius { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        triggerRadius = GetComponent<CircleCollider2D>().radius * transform.localScale.magnitude;
        Debug.Log("TRIGGER RADIUS IS " + triggerRadius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER ENTER");
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        //can connect is true if the ai hasnt been connected with yet
        bool canConnect = characterObject.GetComponent<ConnectionManager>().CanConnect();
        bool connecting = characterObject.GetComponent<ConnectionManager>().Connecting();

        if (player != null && canConnect)
        {
            player.GetConnectTarget(this);
            onInteract.Invoke();

            //set emote target
            characterObject.GetComponent<ConnectionManager>().StartConnection(true,player);
        }
        else if(other.tag == "AIGuy" && !canConnect && !connecting)
        {
            //make other ai snap to position
            other.GetComponent<ConnectionManager>().GetConnectTarget(this);
            other.GetComponent<ConnectionManager>().TrySnapTarget();

            //make both ai enter connection mode
            characterObject.GetComponent<ConnectionManager>().StartConnection(false, player);
            other.GetComponent<ConnectionManager>().StartConnection(false, player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.RemoveConnectTarget(this);
        }
    }
}
