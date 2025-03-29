using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    Moving,
    //Idle,
    Fishing,
    //OnTheHook,
}
public class PlayerStateMachine : BaseStateMachine<PlayerState>
{
    [SerializeField] private InputManager _inputManager;
    public InputManager GetInputManager(){
        return _inputManager;
    }
    protected void Awake()
    {
        //add the states to the statemachine
        States.Add(PlayerState.Moving, new MovingState(this));
        States.Add(PlayerState.Fishing, new FishingState(this));
        //States.Add(PlayerState.OnTheHook, new OnTheHookState(this));
        //set the default state to start on
        CurrentState = States[PlayerState.Moving];
    }
}
