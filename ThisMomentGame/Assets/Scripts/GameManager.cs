using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    { 
        get 
        { 
            if (instance == null) instance = new GameManager();

            return instance; 
        } 
    }

    public int colorAssignments { get; private set; } = 0;

    private int npcCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Get all NPCs

        CharacterAgent[] agents = FindObjectsByType<CharacterAgent>(FindObjectsSortMode.None);
        npcCount = agents.Length;

        Debug.Log("Game manager found " +  npcCount + " NPCs");
    }

    public void IncrementColorAssignments()
    {
        colorAssignments++;

        Debug.Log("Game manager: color assignments: " + colorAssignments);

        if (colorAssignments == npcCount) AllNpcsAssigned();
    }

    public void AllNpcsAssigned()
    {
        Debug.Log("Game manager win state: All NPCs have been assigned a color");
    }
}
