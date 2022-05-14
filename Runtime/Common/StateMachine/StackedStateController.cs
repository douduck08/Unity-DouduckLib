using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public sealed class StackedStateController : IStateController {
        Stack<StateBase> stateStack;
        public StateBase currentState {
            get {
                if (stateStack.Count > 0)
                    return stateStack.Peek ();
                else
                    return null;
            }
        }

        public StackedStateController () {
            stateStack = new Stack<StateBase> ();
        }
        public StackedStateController (StateBase startState) {
            stateStack = new Stack<StateBase> ();
            SetState (startState);
        }

        public void SetState (StateBase state) {
            if (stateStack.Count > 0) {
                stateStack.Pop ();
            }
            if (state != null) {
                state.Reset (this);
                stateStack.Push (state);
            }
        }

        public void PushState (StateBase state) {
            if (state == null) {
                return;
            }
            state.Reset (this);
            stateStack.Push (state);
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