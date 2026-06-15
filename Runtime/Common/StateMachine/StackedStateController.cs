using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public sealed class StackedStateController : IStateController
    {
        Stack<IState> _stateStack;
        public IState CurrentState
        {
            get
            {
                if (_stateStack.Count > 0)
                    return _stateStack.Peek();
                else
                    return null;
            }
        }

        public StackedStateController()
        {
            _stateStack = new Stack<IState>();
        }
        public StackedStateController(IState startState)
        {
            _stateStack = new Stack<IState>();
            SetState(startState);
        }

        public void SetState(IState state)
        {
            if (_stateStack.Count > 0)
            {
                _stateStack.Pop();
            }
            if (state != null)
            {
                state.Reset(this);
                _stateStack.Push(state);
            }
        }

        public void PushState(IState state)
        {
            if (state == null)
            {
                return;
            }
            state.Reset(this);
            _stateStack.Push(state);
        }

        public void StateUpdate()
        {
            var current = CurrentState;
            if (current != null)
            {
                current.StateUpdate();
                if (current.IsCompleted)
                {
                    SetState(current.GetNextState());
                }
            }
        }
    }
}
