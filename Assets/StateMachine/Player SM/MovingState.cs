using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LLB.Statemachine
{
public class MovingState : PlayerBaseState
{

    public MovingState(PlayerStateMachine stateMachine) : base(stateMachine, PlayerState.Moving)
    {
    }

    public override void EnterState()
    {
        //set the input action map to the moving one
        InputManager.Instance.SetInputActionMap(InputManager.Moving);
        Debug.Log("Moving State Entered");
    }
   
    public override void ExitState()
    {

    }

    public override PlayerState GetNextState()
    {
        return PlayerState.Moving;
    }


    public override void UpdateState()
    {
        base.UpdateState();
    }

}
}
