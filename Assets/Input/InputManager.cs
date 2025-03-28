using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Input Manager to push input events to handle contextual input
/// </summary>
public class InputManager : MonoBehaviour
{
    #region Events
    InputAction _moveIA; 
    [SerializeField] PlayerInput _playerInput; 
    //move action
    public static Action<Vector2> OnMove; 
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupInputs();
        SubInputsToEvents(); 
         
    }

    void SetupInputs()
    {
        _moveIA = _playerInput.actions["Move"]; 

    }
    void SubInputsToEvents(){
        _moveIA.performed +=c => OnMove?.Invoke(c.ReadValue<Vector2>());  
    }

}
