using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    public Transform SnapPoint;
    public GameObject characterObject;

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

            //set emote target
            characterObject.GetComponent<ConnectionManager>().StartConnection(other.gameObject);
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
