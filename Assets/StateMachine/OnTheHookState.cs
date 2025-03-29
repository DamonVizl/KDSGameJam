using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnTheHookState : PlayerBaseState
{ 

    public OnTheHookState(PlayerStateMachine stateMachine): base (stateMachine, PlayerState.OnTheHook)
    {
    }

    public override void EnterState()
    {
        //set the input action map to the fishing one
        _inputManager.SetInputActionMap(InputManager.OnTheHook);
        Debug.Log("On the hook State Entered");
    }


    public override void ExitState()
    {

    }

    public override PlayerState GetNextState()
    {
        return PlayerState.OnTheHook;
    }


    public override void UpdateState()
    {
        base.UpdateState();
    }
}