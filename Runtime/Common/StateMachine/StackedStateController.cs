using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public sealed class StackedStateController : IStateController
    {
        Stack<IState> stateStack;
        public IState currentState
        {
            get
            {
                if (stateStack.Count > 0)
                    return stateStack.Peek();
                else
                    return null;
            }
        }

        public StackedStateController()
        {
            stateStack = new Stack<IState>();
        }
        public StackedStateController(IState startState)
        {
            stateStack = new Stack<IState>();
            SetState(startState);
        }

        public void SetState(IState state)
        {
            if (stateStack.Count > 0)
            {
                stateStack.Pop();
            }
            if (state != null)
            {
                state.Reset(this);
                stateStack.Push(state);
            }
        }

        public void PushState(IState state)
        {
            if (state == null)
            {
                return;
            }
            state.Reset(this);
            stateStack.Push(state);
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
