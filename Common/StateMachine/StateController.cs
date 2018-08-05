using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckLib {
    public sealed class StateController : IStateController {
        IState _currentState = null;
        public IState currentState {
            get {
                return _currentState;
            }
        }

        public StateController () { }
        public StateController (IState startState) {
            SetState (startState);
        }

        public void SetState (IState state) {
            _currentState = state;
        }

        public void StateUpdate () {
            if (_currentState != null) {
                _currentState.StateUpdate ();
                if (_currentState.isCompleted) {
                    _currentState = _currentState.GetNextState ();
                }
            }
        }
    }
}