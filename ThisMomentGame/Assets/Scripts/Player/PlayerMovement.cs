using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Variables")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float maxSpeed = 2f;

    float baseDrag;

    [Header("Magnet Variables")]
    [SerializeField] float magnetRadius = 5f;
    [SerializeField] float magnetAngle = 0.3f;

    [SerializeField] float maxMagnetPull = 2f;
    [SerializeField] float currentMagnetPull = 0f;
    [SerializeField] float magnetDrag = 0.3f;

    GameObject magnetTarget = null;

    [Header("Snap Variables")]
    [SerializeField] float distToHardSnap = 0.25f; // this is how far the player has to be from a snap point to snap to it
    [SerializeField] float snappingMagnetMultiplier = 0.5f; // while snapping, speed from the magnet is multiplied by this much
    bool currentlySnapping = false;

    [Header("Object Hookups")]
    [SerializeField] EmoteHandler emoteHandler;

    public PlayerActions actions { get; private set; }
    PlayerActions.PlayerMovementActions playerMovement;

    Rigidbody2D rb;

    Vector2 moveInput;

    ConnectionPoint connectTarget;
    
    // Start is called before the first frame update
    void Awake()
    {
        actions = new PlayerActions();
        actions.Enable();

        playerMovement = actions.PlayerMovement;

        playerMovement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput = 
        playerMovement.Connect.performed += ctx => TrySnapTarget();

        rb = GetComponent<Rigidbody2D>();
        baseDrag = rb.drag;
    }

    private void FixedUpdate()
    {
        currentMagnetPull = 0;
        if (magnetTarget != null)
        {
            // so basically the magnet pull is an extra maximum force addition 
            // we're calculating it here based on how close the player is to the target
            currentMagnetPull = Mathf.Lerp(0, maxMagnetPull, 
                (1 - Vector3.Distance(transform.position, magnetTarget.transform.position) / magnetRadius));
        }

        Move();

        // this is from ChatGPT
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle; // This directly sets Rigidbody2D's rotation
    }

    public void Move()
    {
        if (moveInput != Vector2.zero)
        {
            //Debug.Log("MOVE IS " + moveInput);
        }

        float speedClamp = maxSpeed;

        // These if statements are making redirecting movement go faster
        // so if you are moving left, then swap to moving right, you'll switch velocities super fast
        // should be snappy but not jarring
        if ((moveInput.x > 0 && rb.velocity.x < 0) || (moveInput.x < 0 && rb.velocity.x > 0))
        {
            rb.velocity.Set(moveInput.x * moveSpeed, rb.velocity.y);
        }
        if ((moveInput.y > 0 && rb.velocity.y < 0) || (moveInput.y < 0 && rb.velocity.y > 0))
        {
            rb.velocity.Set(rb.velocity.x, moveInput.y * moveSpeed);
        }

        if (!currentlySnapping)
        {
            // doing the actual movement
            rb.AddForce(moveInput * moveSpeed);

            speedClamp += currentMagnetPull;
            // our deceleration is handled with the Rigidbody's Linear Drag value right now. 
        }
        else
        {
            //currentMagnetPull = 0f;
            Debug.Log(Vector2.Distance(connectTarget.SnapPoint.position, transform.position));

            if (Vector2.Distance(connectTarget.SnapPoint.position, transform.position) <= distToHardSnap)
            {
                rb.velocity = Vector2.zero;
                transform.position = connectTarget.SnapPoint.position;
            }
            else
            {
                Vector2 snapDir = (connectTarget.SnapPoint.position - transform.position).normalized;

                rb.AddForce(snapDir * moveSpeed);

                speedClamp += Mathf.Lerp(0, currentMagnetPull, (Vector2.Distance(transform.position, connectTarget.SnapPoint.position) / connectTarget.triggerRadius));
                //speedClamp += (currentMagnetPull * snappingMagnetMultiplier);
            }
            
        }

        // clamp to a max speed to keep the acceleration on the move without a big mess
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, (speedClamp));
    }

    // these two functions set a magnet for the player that they will basically gravitate towards if they lean into it
    public void GetMagnetTarget(GameObject target)
    {
        magnetTarget = target;
        rb.drag = magnetDrag;
    }

    public void RemoveMagnetTarget(GameObject target)
    {
        if (magnetTarget == target)
        {
            magnetTarget = null;
            rb.drag = baseDrag;
        }
    }

    // these two functions are for our actual connect mechanic.
    // When the player enters a ConnectionPoint near an AI, they become ready to connect
    public void GetConnectTarget(ConnectionPoint connectPoint)
    {
        Debug.Log("SETTING TARGET");
        connectTarget = connectPoint;
    }

    public void RemoveConnectTarget(ConnectionPoint point)
    {
        if (connectTarget == point && !currentlySnapping)
        {
            connectTarget = null;
        }
    }

    // this function snaps the player to the target so that we can run the animation sequence
    void TrySnapTarget()
    {
        Debug.Log("TRY SNAP!");
        if (connectTarget != null)
        {
            //transform.position = connectTarget.SnapPoint.position;

            // snap logic here
            currentlySnapping = true;

            emoteHandler.emoteTarget = connectTarget.characterObject;

            // So normally we should allow the player to run some sort of emote logic before ending the emote
            Invoke("EndEmote", 1f);
            
        }
        else
        {
            EndEmote();
        }
    }

    void EndEmote()
    {
        currentlySnapping = false;
        emoteHandler.emoteTarget = null;
    }
}
