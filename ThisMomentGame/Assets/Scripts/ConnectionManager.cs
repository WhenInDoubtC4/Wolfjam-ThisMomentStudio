using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    private static int totalConnections;

    [SerializeField] private float zoomIncreaseAmount;
    [SerializeField] private float vingnetteIncreaseAmount;
    [SerializeField] private Color[] emoteColors;

    private bool connectionMode;
    private bool connectedBefore;

    //change to enum
    private EmoteEnum requiredEmote;
    Animator emoteAnimator;

    //change this to be a list of enum
    private List<EmoteEnum> triedEmotes;

    private EmoteEnum lastTriedEmote;
    

    // Start is called before the first frame update
    void Start()
    {
        emoteAnimator = GetComponent<Animator>();
        SetRequiredEmote();
    }
    private void SetRequiredEmote()
    {
        //change this to set the emote enum
        int emoteNum = Random.Range(0, 3);
    }

    public bool CanConnect()
    {
        return !connectedBefore;
    }

    //functions returns true if connection can be started, false if it can't be started
    public void StartConnection(bool player)
    {
        if(player && !connectedBefore)
        {
            connectionMode = true;
            triedEmotes = new List<EmoteEnum>();
        }
        else if(!player && connectedBefore)
        {
            PerformEmote(requiredEmote);
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
            Debug.Log(emote);
            if(!connectedBefore)
            {
                if (emote == requiredEmote)
                {
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
    }
    void SetAnimValue(string name, bool value)
    {
        emoteAnimator.SetBool(name, value);
    }

    //this should be called at a certain point in the emote animation
    public void FinishConnection()
    {
        //spriteColor = emoteColors[lastTriedEmote];

        connectionMode = false;
        connectedBefore = true;

        totalConnections++;
        //give camera total number of connections
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
        //do whatever is needed to display the cloud thing
    }
}
