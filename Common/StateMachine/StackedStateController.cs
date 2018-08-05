using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DouduckLib {
    public sealed class StackedStateController : IStateController {

        IState _currentState = null;
        public IState currentState {
            get {
                return _currentState;
            }
        }

        public StackedStateController () { }
        public StackedStateController (IState startState) {
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

        // public override void PopState (IState state) {
        //     if (state == null) {
        //         return;
        //     }

        //     if (!m_stateStack.Contains (state)) {
        //         return;
        //     }

        //     IState popedItem;
        //     do {
        //         popedItem = m_stateStack.Pop ();
        //         m_endingStates.Enqueue (popedItem);
        //     } while (popedItem != state);
        //     m_currentState = m_stateStack.Peek ();
        // }

        // public override void PushState (IState state) {
        //     if (state == null) {
        //         return;
        //     }
        //     state.SetProperty (this, m_stateStack.Count);
        //     m_currentState = state;
        //     m_stateStack.Push (state);
        // }
    }
}