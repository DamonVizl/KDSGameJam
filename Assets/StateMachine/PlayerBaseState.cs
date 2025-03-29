using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerBaseState : BaseState<PlayerState>
{
    #region External Component References
    protected PlayerStateMachine _playerStateMachine;
    protected InputManager _inputManager;

    #endregion



    public PlayerBaseState(PlayerStateMachine stateMachine, PlayerState stateKey) : base(stateKey)
    {
        _playerStateMachine = stateMachine;    
        _inputManager = stateMachine.GetInputManager();  
        if(_inputManager == null)
        {
            Debug.LogError("InputManager not found in parent of PlayerStateMachine, attach the");
        }

    }

    public override void UpdateState()
    {
    }
}

