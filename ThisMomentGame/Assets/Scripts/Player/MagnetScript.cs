using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetScript : MonoBehaviour
{
    PlayerMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AIGuy"))
        {
            Debug.Log("ADDING MAG TARGET");
            movement.GetMagnetTarget(other.gameObject);
        }
       
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("AIGuy"))
        {
            Debug.Log("BYE MAG TARGET");
            movement.RemoveMagnetTarget(other.gameObject);
        }
    }
}
