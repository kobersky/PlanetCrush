using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* Displayed when the game is over */
public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _subMessage;
    [SerializeField] private Button _continueButton;

    public static event Action OnGameOverApproved;

    private void Awake()
    {
        _continueButton.onClick.AddListener(OnContinueClicked);
    }

    public void InitMessage(string message)
    {
        _subMessage.text = message;
    }

    public void OnContinueClicked()
    {
        OnGameOverApproved?.Invoke();
    }

    private void OnDestroy()
    {
        _continueButton.onClick.RemoveAllListeners();
    }
}
