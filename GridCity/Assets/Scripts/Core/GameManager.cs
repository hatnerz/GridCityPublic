using Assets.Scripts.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        LevelSelect,
        Gameplay,
        Paused,
        MultiplayerHub,
        MultiplayerGameplay
    }

    private readonly Dictionary<GameState, string> SceneNames = new()
    {
        { GameState.MainMenu, nameof(GameState.MainMenu) },
        { GameState.LevelSelect, nameof(GameState.LevelSelect) },
        { GameState.Gameplay, nameof(GameState.Gameplay) },
        { GameState.MultiplayerHub, nameof(GameState.MultiplayerHub) },
        { GameState.MultiplayerGameplay, nameof(GameState.MultiplayerGameplay) }
    };

    public GameState CurrentGameState { get; private set; }

    [SerializeField] private float sceneTransitionTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // TODO: add anim to load screen;
        yield return new WaitForSeconds(sceneTransitionTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void StartLevel(int levelNumber)
    {
        Assets.Scripts.Core.GameState.CurrentLevelNumber = levelNumber;
        ChangeGameState(GameState.Gameplay);
    }

    public void OpenLevelSelect()
    {
        ChangeGameState(GameState.LevelSelect);
    }

    public void ReturnToMainMenu()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void OpenMultiplayer()
    {
        ChangeGameState(GameState.MultiplayerHub);
    }

    public void StartMultiplayerGame(Guid lobbyId)
    {
        Assets.Scripts.Core.GameState.MultiplayerLobbyId = lobbyId;
        ChangeGameState(GameState.MultiplayerGameplay);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
        LoadScene(SceneNames[newState]);
    }
}