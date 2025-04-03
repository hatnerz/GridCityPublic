using Assets.Scripts.UI.Buttons;
using Assets.Scripts.UI.Buttons.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayHudManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private ActionButton pauseButton;
    [SerializeField] private Canvas hudCanvas;

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        if (pauseButton != null)
        {
            pauseButton.SetAction(new OpenPauseMenuAction(hudCanvas.gameObject, pauseMenuPrefab));
        }
        else
        {
            Debug.LogError("Open Pause Menu Button is not assigned in the inspector");
        }
    }
}