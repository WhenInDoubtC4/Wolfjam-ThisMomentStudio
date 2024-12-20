using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ConnectionManager : MonoBehaviour
{
    public UnityEvent<EmoteEnum> onConnectionFinished = new();

    private static int totalConnections;

    [SerializeField] private float newConnectionDelay;
    private float newConnectionTimer;
    private bool canConnect;


    [SerializeField] private float zoomIncreaseAmount;
    [SerializeField] private float vingnetteIncreaseAmount;
    [SerializeField] private Color[] emoteColors;

    [SerializeField] private ScribbleDisplay displeasureDisplay;

    private PlayerMovement player;

    private bool connectionMode;
    private bool connectedBefore;

    ConnectionPoint connectTarget;
    Rigidbody2D rb;

    bool correctConnectionHappening = false;

    //change to enum
    private EmoteEnum requiredEmote;
    Animator emoteAnimator;

    //change this to be a list of enum
    private List<EmoteEnum> triedEmotes;

    private EmoteEnum lastTriedEmote;

    [Header("Snap Variables")]
    [SerializeField] private float snapMoveSpeed;
    [SerializeField] private float snapMaxSpeed;
    [SerializeField] float distToHardSnap = 0.25f; // this is how far the player has to be from a snap point to snap to it
    [SerializeField] float snapResistance = 0.65f;


    [SerializeField] private float connectionEndDelay;
    private float connectionEndTimer;
    private bool isConnectionEndTimer;

    private float originalXScale;

    private void Update()
    {
        SnapMovement();
        ConnectionDelay();
        ConnectionFinishedDelay();
    }
    // Start is called before the first frame update
    void Start()
    {
        originalXScale = transform.localScale.x;
        emoteAnimator = GetComponent<Animator>();
        canConnect = true;
        rb = GetComponent<Rigidbody2D>();
        //emoteAnimator = GetComponent<Animator>();
        SetRequiredEmote();


    }
    private void SetRequiredEmote()
    {
        //change this to set the emote enum
        requiredEmote = (EmoteEnum)Random.Range(0, 3);
    }

    public bool CanConnect()
    {
        return canConnect;
    }
    public bool ConnectedBefore()
    {
        return connectedBefore;
    }
    public bool Connecting()
    {
        return connectionMode;
    }

    //functions returns true if connection can be started, false if it can't be started
    public void StartConnection(bool isPlayer,PlayerMovement newPlayer)
    {
        if(!connectionMode && canConnect)
        {
            connectionMode = true;
            if (isPlayer)
            {
                player = newPlayer;
                
            }
            
            if (isPlayer && !connectedBefore)
            {
                triedEmotes = new List<EmoteEnum>();
            }
            else if (!isPlayer && connectedBefore)
            {
                Debug.Log("doing emote on ai");

                // WHAT DO HERE???
                //PerformEmote(requiredEmote);
            }
            else if (!player)
            {
                //wait for other ai to perform emote
            }
        }
        
    }
    private void ConnectionDelay()
    {
        if(!canConnect)
        {
            newConnectionTimer -= Time.deltaTime;
            if(newConnectionTimer <= 0)
            {
                canConnect = true;
            }
        }
    }

    //add enum param for emote that is tried
    public void TryEmote(EmoteEnum emote)
    {
        lastTriedEmote = emote;
        if(connectionMode)
        {
            if(!connectedBefore)
            {
                if (emote == requiredEmote)
                {
                    Debug.LogError("correct emote!");
                    correctConnectionHappening = true;

                    isConnectionEndTimer = true;
                    connectionEndTimer = connectionEndDelay;
                    PerformEmote();
                    
                }
                else
                {
                    Debug.LogError("CORRECT IS " + requiredEmote);
                    WrongEmote(emote);
                }
            }
            else
            {
                //this is for when another ai performs an emote and this ai matches it
                //should only get here with ai->ai interaction

                // emote handler do emote?
                //PerformEmote(emote);
            }
        }
    }
    private void PerformEmote()
    {
        switch (requiredEmote)
        {
            case EmoteEnum.Red:
                emoteAnimator.SetTrigger("Red");
                break;
            case EmoteEnum.Green:
                emoteAnimator.SetTrigger("Green");
                break;
            case EmoteEnum.Blue:
                emoteAnimator.SetTrigger("Blue");
                break;
            case EmoteEnum.Yellow:
                emoteAnimator.SetTrigger("Yellow");
                break;
        }
    }
    private void ConnectionFinishedDelay()
    {
        if(isConnectionEndTimer)
        {
            connectionEndTimer -= Time.deltaTime;
            if(connectionEndTimer <= 0)
            {
                isConnectionEndTimer = false;
                FinishConnection();
            }
        }
    }
    public void EmoteFinished()
    {
        if (correctConnectionHappening)
        {
            //FinishConnection();
            correctConnectionHappening = false;
        }        
    }

    //this should be called at a certain point in the emote animation
    public void FinishConnection()
    {
        Debug.Log(lastTriedEmote.ToString());
        onConnectionFinished.Invoke(lastTriedEmote);
        //connectTarget.OnSuccessfulInteract();

        connectTarget = null;

        //begin delay before new connection can be made
        newConnectionDelay = newConnectionTimer;
        canConnect = false;

        //change color of ai
        //spriteColor = emoteColors[lastTriedEmote];
        connectionMode = false;
        connectedBefore = true;

        totalConnections++;
        //give camera total number of connections

        player.EndEmote();

        //add function to tell ai connection is over 
    }

    //pass in enum for tried emote
    private void WrongEmote(EmoteEnum triedEmote)
    {
        if(!triedEmotes.Contains(triedEmote))
        {
            IncreaseTension();
        }
        DisplayDispleasure();
    }
    private void IncreaseTension()
    {
        //call camera function that changes zoom

        //call camera function that changes vingnette
    }
    private void DisplayDispleasure()
    {
        Debug.Log("bad");
        //do whatever is needed to display the cloud thing
        displeasureDisplay.StartEffect();
    }




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
    private void ReachedTarget()
    {
        isConnectionEndTimer = true;
        connectionEndTimer = connectionEndDelay;
        PerformEmote();
    }
    private void SnapMovement()
    {
        if(connectTarget != null)
        {
            if (Vector2.Distance(connectTarget.SnapPoint.position, transform.position) <= distToHardSnap)
            {
                rb.velocity = Vector2.zero;
                transform.position = connectTarget.SnapPoint.position;
                ReachedTarget();
                connectTarget = null;
            }
            else
            {
                Vector2 snapDir = (connectTarget.SnapPoint.position - transform.position).normalized;

                rb.AddForce(snapDir * snapMoveSpeed);

            }

            rb.velocity = Vector2.ClampMagnitude(rb.velocity, (snapMaxSpeed));
        }

    }
}
