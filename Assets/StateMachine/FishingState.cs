using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingState : PlayerBaseState
{ 

    public FishingState(PlayerStateMachine stateMachine): base (stateMachine, PlayerState.Fishing)
    {
    }

    public override void EnterState()
    {
        //set the input action map to the fishing one
        _inputManager.SetInputActionMap(InputManager.Fishing);
        Debug.Log("Fishing State Entered");
    }


    public override void ExitState()
    {

    }

    public override PlayerState GetNextState()
    {
        return PlayerState.Fishing;
    }


    public override void UpdateState()
    {
        base.UpdateState();
    }
}
