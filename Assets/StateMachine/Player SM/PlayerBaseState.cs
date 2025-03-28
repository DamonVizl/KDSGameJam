using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using LLB.Statemachine;

namespace LLB.Statemachine
{
public abstract class PlayerBaseState : BaseState<PlayerState>
{
    #region External Component References
    protected PlayerStateMachine _playerStateMachine;


    #endregion



    public PlayerBaseState(PlayerStateMachine stateMachine, PlayerState stateKey) : base(stateKey)
    {
        _playerStateMachine = stateMachine;       

    }

    public override void UpdateState()
    {
    }
}

}