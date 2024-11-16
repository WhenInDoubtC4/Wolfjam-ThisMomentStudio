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
    private int requiredEmote;

    //change this to be a list of enum
    private List<int> triedEmotes;

    private int lastTriedEmote;


    // Start is called before the first frame update
    void Start()
    {
        SetRequiredEmote();
    }
    private void SetRequiredEmote()
    {
        //change this to set the emote enum
        int emoteNum = Random.Range(0, 3);
    }

    //functions returns true if connection can be started, false if it can't be started
    public bool StartConnection(bool player)
    {
        if(player && !connectedBefore)
        {
            connectionMode = true;
            triedEmotes = new List<int>();
            return true;
        }
        else if(!player && connectedBefore)
        {
            PerformEmote(requiredEmote);
            return true;
        }
        else if(!player)
        {
            //wait for other ai to perform emote
            return true;
        }
        return false;
    }

    //add enum param for emote that is tried
    public void TryEmote(int emote)
    {
        lastTriedEmote = emote;
        if(connectionMode)
        {
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
    private void PerformEmote(int emote)
    {
        //call trigger for emote animation
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
    private void WrongEmote(int triedEmote)
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
