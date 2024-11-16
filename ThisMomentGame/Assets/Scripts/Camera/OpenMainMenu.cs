using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMainMenu : MonoBehaviour
{
    public PlayerActions playerActions;
    PlayerActions.SystemActions system;

    [SerializeField] GameObject mainMenu;
    [SerializeField] FadeUI fadeUI;

    [SerializeField] Transform creditsMenu;
    Vector3 creditsStartPos;

    private void Start()
    {
        playerActions = new PlayerActions();
        playerActions.Enable();
        system = playerActions.System;

        system.MainMenu.performed += ctx => Open();

        creditsStartPos = creditsMenu.position;
    }

    void Open()
    {
        if (!mainMenu.activeInHierarchy)
        {
            mainMenu.SetActive(true);
            fadeUI.FadeIn();

            creditsMenu.GetComponent<WaypointFollower>().MoveToWaypoint(0);
            creditsMenu.position = creditsStartPos;
        }
    }
}
