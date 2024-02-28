using System;
using UnityEngine;

/* AppManager handles flow of game's lifecycle */
public class AppManager : MonoBehaviour
{
    [SerializeField] UIManager _uiManager;
    [SerializeField] GamePlayManager _gameplayManager;
    [SerializeField] GameConfiguration _gameConfiguration;

    private void OnEnable()
    {
        MainMenu.OnNewGame += StartNewGame;
        MainMenu.OnQuit += QuitGame;

        GamePlayManager.OnOutOfMoves += ShowGameOverMenuMenu;

        GameOverMenu.OnGameOverApproved += ShowMainMenu;
    }

    private void OnDisable()
    {
        MainMenu.OnNewGame -= StartNewGame;
        MainMenu.OnQuit -= QuitGame;

        GamePlayManager.OnOutOfMoves -= ShowGameOverMenuMenu;

        GameOverMenu.OnGameOverApproved -= ShowMainMenu;
    }

    private void Awake()
    {
        _gameplayManager.Init(_gameConfiguration);
    }

    private void Start()
    {
        _uiManager.ToggleMainMenu();
    }

    private void ShowMainMenu()
    {
        _uiManager.ToggleGameOverMenu();
        _uiManager.ToggleMainMenu();
    }

    private void ShowGameOverMenuMenu(int score)
    {
        _uiManager.ToggleGameOverMenu(score.ToString());
    }

    private void StartNewGame()
    {
        _uiManager.ToggleMainMenu();
        _gameplayManager.StartNewGame();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
