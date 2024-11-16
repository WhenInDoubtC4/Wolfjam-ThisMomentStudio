using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionManager : MonoBehaviour
{
    private static int totalConnections;

    [SerializeField] private float zoomIncreaseAmount;
    [SerializeField] private float vingnetteIncreaseAmount;
    [SerializeField] private Color[] emoteColors;

    [SerializeField] private ScribbleDisplay displeasureDisplay;

    private PlayerMovement player;

    private bool connectionMode;
    private bool connectedBefore;

    ConnectionPoint connectTarget;
    Rigidbody2D rb;


    //change to enum
    private EmoteEnum requiredEmote;
    Animator emoteAnimator;

    //change this to be a list of enum
    private List<EmoteEnum> triedEmotes;

    private EmoteEnum lastTriedEmote;
    

    // Start is called before the first frame update
    void Start()
    {
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
        return !connectedBefore;
    }
    public bool Connecting()
    {
        return connectionMode;
    }

    //functions returns true if connection can be started, false if it can't be started
    public void StartConnection(bool isPlayer,PlayerMovement newPlayer)
    {
        connectionMode = true;
        if(isPlayer)
        {
            player = newPlayer;
        }
        if(isPlayer && !connectedBefore)
        {
            triedEmotes = new List<EmoteEnum>();
        }
        else if(!isPlayer && connectedBefore)
        {
            Debug.Log("doing emote on ai");
            PerformEmote(requiredEmote);
            FinishConnection();
        }
        else if(!player)
        {
            //wait for other ai to perform emote
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
                    Debug.Log("correct emote!");
                    PerformEmote(requiredEmote);
                }
                else
                {
                    WrongEmote(emote);
                }
            }
            else
            {
                //this is for when another ai performs an emote and this ai matches it
                //should only get here with ai->ai interaction
                PerformEmote(emote);
            }
        }
    }
    private void PerformEmote(EmoteEnum emote)
    {
        SetAnimValue("Red", false);
        SetAnimValue("Green", false);
        SetAnimValue("Blue", false);
        SetAnimValue("Yellow", false);

        switch (emote)
        {
            case EmoteEnum.Red:
                SetAnimValue("Red", true);
                break;
            case EmoteEnum.Green:
                SetAnimValue("Green", true);
                break;
            case EmoteEnum.Blue:
                SetAnimValue("Blue", true);
                break;
            case EmoteEnum.Yellow:
                SetAnimValue("Yellow", true);
                break;
        }


        FinishConnection();
    }
    void SetAnimValue(string name, bool value)
    {
        //emoteAnimator.SetBool(name, value);
    }

    //this should be called at a certain point in the emote animation
    public void FinishConnection()
    {
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

    // this function snaps the player to the target so that we can run the animation sequence
    public void TrySnapTarget()
    {
        Debug.Log("TRY SNAP!");
        if (connectTarget != null)
        {
            Debug.Log("attempting ai snap");
            transform.position = connectTarget.SnapPoint.position;

            // snap logic here
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;


            // So normally we should allow the player to run some sort of emote logic before ending the emote


        }
        
    }
}
