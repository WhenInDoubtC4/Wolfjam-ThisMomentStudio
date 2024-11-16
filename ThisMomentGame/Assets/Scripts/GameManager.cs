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

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //Find all colorless NPCs
    }
}
