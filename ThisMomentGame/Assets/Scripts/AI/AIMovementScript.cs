using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementScript : MonoBehaviour
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

    AudioSource audioSource;

    GameObject magnetTarget = null;

    [Header("Object Hookups")]
    [SerializeField] CharacterAgent agent;

    Rigidbody2D rb;

    Vector2 moveInput;

    ConnectionPoint connectTarget;

    Animator myAnimator;
    private float originalXScale;

    // Start is called before the first frame update
    void Awake()
    {
        originalXScale = transform.localScale.x;
        myAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        baseDrag = rb.drag;

        //audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        agent.targetReached.AddListener(OnTargetReached);
    }

    private void OnTargetReached()
    {
        rb.velocity = Vector2.zero;
        moveInput = Vector2.zero;
    }
    private void FixedUpdate()
    {
        moveInput = agent.aiMoveInput;

        //Debug.Log("MOVE INPUT IS " + moveInput);

        currentMagnetPull = 0;
        if (magnetTarget != null)
        {
            // so basically the magnet pull is an extra maximum force addition 
            // we're calculating it here based on how close the player is to the target
            currentMagnetPull = Mathf.Lerp(0, maxMagnetPull,
                (1 - Vector3.Distance(transform.position, magnetTarget.transform.position) / magnetRadius));
        }

        if (moveInput != Vector2.zero)
        {
            //if (!audioSource.isPlaying)
            //{
            //    audioSource.Play();
            //}
        }
        else
        {
            //audioSource.Stop();
        }


        Move();
    }
    private void UpdateWalk()
    {
        Debug.Log(rb.velocity.magnitude);
        if (rb.velocity.magnitude > 0.1f)
        {
            myAnimator.SetBool("Walking", true);
        }
        else
        {
            myAnimator.SetBool("Walking", false);
        }

        myAnimator.SetBool("WalkingUp", false);
        if (rb.velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(-originalXScale, transform.localScale.y, transform.localScale.z);
        }
        else if (rb.velocity.x > 0.1f)
        {
            transform.localScale = new Vector3(originalXScale, transform.localScale.y, transform.localScale.z);
        }

    }
    public void Move()
    {
        UpdateWalk();

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
        if (connectTarget == point)
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
            transform.position = connectTarget.SnapPoint.position;

            // snap logic here
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            // NEED TO SET TARGETS RIGHT
            //emoteHandler.emoteTarget = connectTarget.characterObject;

            // So normally we should allow the player to run some sort of emote logic before ending the emote
            Invoke("EndEmote", 1f);

        }
        else
        {
            EndEmote();
        }
    }

    public void EndEmote()
    {
        rb.isKinematic = false;
        //emoteHandler.emoteTarget = null;
        // NEED TO SET TARGETS RIGHT
    }
}
