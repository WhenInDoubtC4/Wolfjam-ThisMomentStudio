using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;
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
    [SerializeField] float snapResistance = 0.65f;
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
        UpdateMagnetPull();

        Move();

        SetRotation();
    }

    void UpdateMagnetPull()
    {
        currentMagnetPull = 0;
        if (magnetTarget != null)
        {
            bool connectCheck = connectTarget == null;
            if (!connectCheck)
            {
                //Debug.Log("MAG IS " + magnetTarget + " AND CONNECT IS " + connectTarget.transform.parent.gameObject);
                connectCheck = magnetTarget == connectTarget.transform.parent.gameObject;
            }

            if (IsTargetInView() || connectCheck)
            {
                // so basically the magnet pull is an extra maximum force addition 
                // we're calculating it here based on how close the player is to the target
                currentMagnetPull = Mathf.Lerp(0, maxMagnetPull,
                    (1 - Vector3.Distance(transform.position, magnetTarget.transform.position) / magnetRadius));
            }
        }
    }

    // chatgpt 
    bool IsTargetInView()
    {
        // Get direction to the target
        Vector2 directionToTarget = (Vector2)(magnetTarget.transform.position - transform.position);
        directionToTarget.Normalize();

        // Get the forward direction of the object (assume the object faces right by default in Unity 2D)
        Vector2 forward = transform.right;

        //Debug.Log("FORWARD IS " + forward);
        // Calculate the angle
        float angleToTarget = Vector2.SignedAngle(forward, directionToTarget) - 90f;
        //Debug.Log("ANGLE TO TARGET IS " + angleToTarget);

        // Check if the angle is within the view cone
        return Mathf.Abs(angleToTarget) <= magnetAngle;
    }

    void Move()
    {
        if (moveInput != Vector2.zero)
        {
            //Debug.Log("MOVE IS " + moveInput);
        }

        float speedClamp = maxSpeed;

        // -------------------- SMOOTH INVERSION ----------------------- \\
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
            // -------------------- PLAYER MOVEMENT ----------------------- \\

            // doing the actual movement
            rb.AddForce(moveInput * moveSpeed);

            speedClamp += currentMagnetPull;
            // our deceleration is handled with the Rigidbody's Linear Drag value right now. 
        }
        else
        {
            // -------------------- SNAPPING MOVEMENT ----------------------- \\

            //Debug.Log(Vector2.Distance(connectTarget.SnapPoint.position, transform.position));

            if (Vector2.Distance(connectTarget.SnapPoint.position, transform.position) <= distToHardSnap)
            {
                rb.velocity = Vector2.zero;
                transform.position = connectTarget.SnapPoint.position;
            }
            else
            {
                Vector2 snapDir = (connectTarget.SnapPoint.position - transform.position).normalized;

                rb.AddForce(snapDir * moveSpeed);
                rb.AddForce(moveInput * snapResistance * moveSpeed);

                // so while we're actively snapping to the target, we're reducing the force of the magnet pull
                // based on the distance of the player to the snap point in the context of the sphere
                // so as they get closer they go slower
                // but it should be smooth and interpolated based on the player's movement right before
                speedClamp += Mathf.Lerp(0, currentMagnetPull,
                    (Vector2.Distance(transform.position, connectTarget.SnapPoint.position) / connectTarget.triggerRadius));
            }

        }

        // -------------------- CLAMP SPEED ----------------------- \\

        // clamp to a max speed to keep the acceleration on the move without a big mess
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, (speedClamp));
    }

    // this is from ChatGPT
    void SetRotation()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90f; // except for the -90 degree part that was a bugfix by Mason
        rb.rotation = angle; // This directly sets Rigidbody2D's rotation
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
            //Invoke("EndEmote", 1.2f);

        }
        else
        {
            EndEmote();
        }
    }

    public void EndEmote()
    {
        currentlySnapping = false;
        emoteHandler.emoteTarget = null;
    }
}