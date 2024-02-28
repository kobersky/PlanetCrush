using System;
using UnityEngine;
using UnityEngine.InputSystem;

/* responsible for detecting player's attempt to switch between 2 planets*/
public class InputManager : MonoBehaviour
{
    private InputActionsConfig _inputActionsConfig;

    private bool _isPressed;
    private IClickable _originClick;

    public static event Action <IClickable, IClickable> OnClickablesChosenByPlayer;

    private void Awake()
    {
        _inputActionsConfig = new InputActionsConfig();
    }

    private void OnEnable()
    {
        EnablePlayerInput();
    }

    private void OnDisable()
    {
        DisablePlayerInput();
    }

    /* when a click is detected, store the clickable*/
    private void OnClickStarted(InputAction.CallbackContext context)
    {
        if (_isPressed) return;
        _isPressed = true;
        _originClick = FindPointedClickable();
    }

    /*if click is pressed, keep trying to detect if pointer hovers above another clickable.
     * once another clickable is detected, release press and report origin and target clickables */
    private void Update()
    {
        if (_isPressed)
        {
            var targetClick = FindPointedClickable();
            var areCliclablesDifferent = _originClick != null && targetClick != null && targetClick != _originClick;
            if (areCliclablesDifferent)
            {
                Debug.Log($"InputManager: clickables chosen: {_originClick}, {targetClick}");
                OnClickablesChosenByPlayer(_originClick, targetClick);
                ResetClick();
            }
        }
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        ResetClick();
    }

    private IClickable FindPointedClickable()
    {
        var pointerPosition = _inputActionsConfig.Player.Point.ReadValue<Vector2>();

        var ray = Camera.main.ScreenPointToRay(pointerPosition);         

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent<IClickable>(out var foundClickable))
            {
                return foundClickable;
            }
        }

        return null;
    }

    private void ResetClick()
    {
        _isPressed = false;
        _originClick = null;
    }

    public void EnablePlayerInput()
    {
        _inputActionsConfig.Enable();
        _inputActionsConfig.Player.Click.started += OnClickStarted;
        _inputActionsConfig.Player.Click.canceled += OnClickCanceled;
    }

    public void DisablePlayerInput()
    {
        _inputActionsConfig.Player.Click.started -= OnClickStarted;
        _inputActionsConfig.Player.Click.canceled -= OnClickCanceled;
        _inputActionsConfig.Disable();
    }
}
