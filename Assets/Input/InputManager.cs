using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static readonly string Moving = "Player";
    public static readonly string Fishing = "Fishing";
    public static readonly string OnTheHook = "OnTheHook";

    [SerializeField] private PlayerInput _input;
    private InputAction _castIA, _recallIA, _reelIA;

    #region Events
    public Action OnCastPressed, OnCastReleased,OnRecallPressed, OnReelPressed, OnReelReleased;
    #endregion

    void OnEnable()
    {
        SetupInputActions();
    }
    void OnDisable()
    {
        CleanUpInputActions();  
    }

    void Update()
    {

    }

    #region Setup Input Actions

    private void SetupInputActions()
    {
        _castIA = _input.actions["Cast"];
        _castIA.performed += c => OnCastPressed?.Invoke();
        _castIA.canceled += c => OnCastReleased?.Invoke();

        _recallIA = _input.actions["Recall"];
        _recallIA.performed += c => OnRecallPressed?.Invoke();

        _reelIA = _input.actions["Reel"];
        _reelIA.performed += c => OnReelPressed?.Invoke();
        _reelIA.canceled += c => OnReelReleased?.Invoke();

    }

    private void CleanUpInputActions()
    {
        _castIA.performed -= c => OnCastPressed?.Invoke();
        _castIA.canceled -= c => OnCastReleased?.Invoke();

        _recallIA.performed -= c => OnRecallPressed?.Invoke();

        _reelIA.performed -= c => OnReelPressed?.Invoke();
        _reelIA.canceled -= c => OnReelReleased?.Invoke();
    }


    #endregion

    /// <summary>
    /// switch the input action map so the input does different things, used from the state machine 
    /// </summary>
    /// <param name="actionMap"></param>
    public void SetInputActionMap(string actionMap)
    {
        _input.SwitchCurrentActionMap(actionMap);
    }


}