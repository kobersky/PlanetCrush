using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] GameOverMenu _gameOverMenu;

    [SerializeField] MovesLeftDisplay _movesLeftDisplay;
    [SerializeField] ScoreDisplay _scoreDisplay;

    [SerializeField] EndRoundMessage _endRoundMessage;
    [SerializeField] ReshufflingMessage _reshufflingMessage;
    [SerializeField] SpecialMessagePool _specialMessagePool;

    private const int MIN_MATCH_TO_DISPLAY = 2;


    public void ToggleMainMenu()
    {
        _mainMenu.gameObject.SetActive(!_mainMenu.gameObject.activeInHierarchy);
    }

    public void ToggleGameOverMenu(string message = null)
    {
        _gameOverMenu.InitMessage(message);
        _gameOverMenu.gameObject.SetActive(!_gameOverMenu.gameObject.activeInHierarchy);
    }

    public void ResetScoreDisplay()
    {
        _scoreDisplay.UpdateDisplay(0);
    }


    public void RefreshScoreDisplay(int prevScore, int newScore)
    {
        _scoreDisplay.UpdateDisplay(prevScore, newScore);
    }

    public void RefreshMovesLeftDisplay(int movesLeft)
    {
        _movesLeftDisplay.UpdateDisplay(movesLeft);
    }

    public void DisplaySpecialMatchMessage(Vector3 position, SpecialMatchType specialMatchType)
    {
        var message = _specialMessagePool.GetMessage();
        message.DisplayMessage(position, specialMatchType, OnDoneDisplayingSpecialMessage);
    }

    public void OnDoneDisplayingSpecialMessage(SpecialMatchMessage specialMatchMessage)
    {
        _specialMessagePool.ReturnMessageToPool(specialMatchMessage);
    }

    public void DisplayTotalRoundMessages(int matchCounter)
    {
        if (matchCounter < MIN_MATCH_TO_DISPLAY) return;

        _endRoundMessage.DisplayMessage(matchCounter);
    }

    public void DisplayReshufflingMessage()
    {
        _reshufflingMessage.DisplayMessage();
    }
}
