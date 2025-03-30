using System.Collections.Generic;
using UnityEngine;
using System;


    /*the abstract statemachine to build other statemachines in the game off.
     * has a Dictionary of State keys and states (called BaseState, from the class) 
     * takes a generic which has to be an enum (these will be the possible states of the statemachine)
     * 
     * 
     * */
    public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
    {

        //this is a dictionary of states that the state machine will have. you'll be able to get the state by calling the dictionary with it's State Enum (EState)
        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
        //This is the statemachines current state
        protected BaseState<EState> CurrentState;// 
        private void Start()
        {
            CurrentState.EnterState();
        }
        private void Update()
        {
            EState nextStateKey = CurrentState.GetNextState();
            if (nextStateKey.Equals(CurrentState.StateKey))
            {
                CurrentState.UpdateState();
            }
            else
            {
                TransitionToState(nextStateKey);
            }
        }
        public void TransitionToState(EState stateKey)
        {
            CurrentState.ExitState();
            CurrentState = States[stateKey];
            CurrentState.EnterState();
        }
        public EState GetCurrentState()
        {
            return CurrentState.StateKey;
        }


    }
