using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckLib {
    public sealed class StateController : IStateController {
        StateBase _currentState = null;
        public StateBase currentState {
            get {
                return _currentState;
            }
        }

        public StateController () { }
        public StateController (StateBase startState) {
            SetState (startState);
        }

        public void SetState (StateBase state) {
            if (state != null) {
                state.Reset (this);
                _currentState = state;
            }
        }

        public void StateUpdate () {
            var current = currentState;
            if (current != null) {
                current.StateUpdate ();
                if (current.isCompleted) {
                    SetState (current.GetNextState ());
                }
            }
        }
    }
}