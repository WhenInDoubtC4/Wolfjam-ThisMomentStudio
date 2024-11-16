using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float maxSpeed = 2f;

    PlayerActions actions;
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
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        if (moveInput != Vector2.zero)
        {
            Debug.Log("MOVE IS " + moveInput);
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
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);     

        // our deceleration is handled with the Rigidbody's Linear Drag value right now. 

    }


}
