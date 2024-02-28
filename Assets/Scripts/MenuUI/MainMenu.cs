using System;
using UnityEngine;
using UnityEngine.UI;

/* Displayed when the game launched/restarted */
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _quitButton;

    public static event Action OnNewGame;
    public static event Action OnQuit;

    private void Awake()
    {
        _newGameButton.onClick.AddListener(OnNewGameClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void OnNewGameClicked()
    {
        OnNewGame?.Invoke();
    }

    public void OnQuitClicked()
    {
        OnQuit?.Invoke();
    }

    private void OnDestroy()
    {
        _newGameButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
    }
}
