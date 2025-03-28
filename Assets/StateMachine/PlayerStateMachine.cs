using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LLB.Statemachine
{
public enum PlayerState
{
    Moving,
    //Idle,
    Fishing,
    OnTheHook,
}
public class PlayerStateMachine : BaseStateMachine<PlayerState>
{
    public static PlayerStateMachine Instance { get; private set; } //this is a singleton instance of the statemachine. it will be used to access the statemachine from other classes


    protected void Awake()
    {
        //setup the singleton instance
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //add the states to the statemachine
        States.Add(PlayerState.Moving, new MovingState(this));
        States.Add(PlayerState.Fishing, new FishingState(this));
        States.Add(PlayerState.OnTheHook, new OnTheHookState(this));
        //set the default state to start on
        CurrentState = States[PlayerState.Moving];
    }
}
}

