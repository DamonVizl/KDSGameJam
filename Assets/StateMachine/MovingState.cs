using UnityEngine;


public class MovingState : PlayerBaseState
{

    public MovingState(PlayerStateMachine stateMachine) : base(stateMachine, PlayerState.Moving)
    {
    }

    public override void EnterState()
    {
        //set the input action map to the moving one
        _inputManager.SetInputActionMap(InputManager.Moving);
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
