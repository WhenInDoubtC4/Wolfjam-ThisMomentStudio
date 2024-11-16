using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConnectionPoint : MonoBehaviour
{
    public Transform SnapPoint;
    public GameObject characterObject;
    public UnityEvent onInteract= new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER ENTER");
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        bool canConnect = characterObject.GetComponent<ConnectionManager>().CanConnect();

        if (player != null && canConnect)
        {
            player.GetConnectTarget(this);
            onInteract.Invoke();

            //set emote target
            characterObject.GetComponent<ConnectionManager>().StartConnection(other.gameObject,player);
        }
        else if(other.tag == "AIGuy")
        {
            characterObject.GetComponent<ConnectionManager>().GetConnectTarget(this);
            characterObject.GetComponent<ConnectionManager>().TrySnapTarget();
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
