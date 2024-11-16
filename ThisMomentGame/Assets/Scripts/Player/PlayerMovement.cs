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

    public PlayerActions actions { get; private set; }
    PlayerActions.PlayerMovementActions playerMovement;

    Rigidbody2D rb;

    Vector2 moveInput;
    
    // Start is called before the first frame update
    void Awake()
    {
        actions = new PlayerActions();
        actions.Enable();

        playerMovement = actions.PlayerMovement;

        playerMovement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput = 

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
    }

    public void Move()
    {
        if (moveInput != Vector2.zero)
        {
            //Debug.Log("MOVE IS " + moveInput);
        }

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

        // doing the actual movement
        rb.AddForce(moveInput * moveSpeed);

        // clamp to a max speed to keep the acceleration on the move without a big mess
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, (maxSpeed + currentMagnetPull));     

        // our deceleration is handled with the Rigidbody's Linear Drag value right now. 
    }

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
}
