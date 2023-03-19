using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckLib
{
    public sealed class StateController : IStateController
    {
        IState _currentState = null;
        public IState currentState
        {
            get
            {
                return _currentState;
            }
        }

        public StateController() { }
        public StateController(IState startState)
        {
            SetState(startState);
        }

        public void SetState(IState state)
        {
            if (state != null)
            {
                state.Reset(this);
                _currentState = state;
            }
        }

        public void StateUpdate()
        {
            var current = currentState;
            if (current != null)
            {
                current.StateUpdate();
                if (current.isCompleted)
                {
                    SetState(current.GetNextState());
                }
            }
        }
    }
}