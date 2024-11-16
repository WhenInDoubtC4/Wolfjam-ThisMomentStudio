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
        //Debug.Log("TRIGGER RADIUS IS " + triggerRadius);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("TRIGGER ENTER");
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        //can connect is true if the ai hasnt been connected with yet
        bool connectedBefore = characterObject.GetComponent<ConnectionManager>().ConnectedBefore();
        bool canConnect = characterObject.GetComponent<ConnectionManager>().CanConnect();
        bool connecting = characterObject.GetComponent<ConnectionManager>().Connecting();
        bool otherCanConnect = true;
        bool otherConnecting = false;
        if (other.tag == "AIGuy")
        {
            Debug.Log("is ai");
            otherConnecting = other.GetComponent<ConnectionManager>().Connecting();
            otherCanConnect = other.GetComponent<ConnectionManager>().CanConnect();
        }
        Debug.Log(" connecting: " + connecting + " can connect " + canConnect + " other connecting " + otherConnecting + " other can connect " + otherCanConnect + " connected before " + connectedBefore);
        if (player != null && !connectedBefore)
        {
            Debug.Log("player connect");
            player.GetConnectTarget(this);
            onInteract.Invoke();

            //set emote target
            characterObject.GetComponent<ConnectionManager>().StartConnection(true,player);
        }
        else if(other.tag == "AIGuy" && canConnect && otherCanConnect && !connecting && !otherConnecting && connectedBefore)
        {
            Debug.Log("hit");
            //make ai snap to position
            characterObject.GetComponent<ConnectionManager>().GetConnectTarget(other.transform.GetChild(0).GetComponent<ConnectionPoint>());
            characterObject.GetComponent<Rigidbody2D>().isKinematic = false;
            characterObject.GetComponent<Rigidbody2D>().gravityScale = 0;

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
