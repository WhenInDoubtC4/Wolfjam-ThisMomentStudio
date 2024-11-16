using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMainMenu : MonoBehaviour
{
    public PlayerActions playerActions;
    PlayerActions.SystemActions system;

    [SerializeField] GameObject mainMenu;
    [SerializeField] FadeUI fadeUI;

    private void Start()
    {
        playerActions = new PlayerActions();
        playerActions.Enable();
        system = playerActions.System;

        system.MainMenu.performed += ctx => Open();
    }

    void Open()
    {
        if (!mainMenu.activeInHierarchy)
        {

            mainMenu.SetActive(true);
            fadeUI.FadeIn();
        }
    }
}
