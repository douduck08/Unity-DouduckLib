﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public interface IStateController
    {
        IState currentState { get; }
        void SetState(IState state);
        void StateUpdate();
    }

    public interface IState
    {
        IStateController controller { get; }
        bool isStarted { get; }
        bool isCompleted { get; }
        void Reset(IStateController controller);
        void StateUpdate();
        IState GetNextState();
    }

    public abstract class StateBase : IState
    {
        public IStateController controller => _controller;
        public bool isStarted => _isStarted;
        public bool isCompleted => _isCompleted;

        IStateController _controller;
        bool _isStarted = false;
        bool _isCompleted = false;

        public void Reset(IStateController controller)
        {
            _controller = controller;
            _isStarted = false;
            _isCompleted = false;
        }

        public void StateUpdate()
        {
            if (!_isStarted)
            {
                _isStarted = true;
                OnStateEnter();
            }
            if (!_isCompleted)
            {
                OnStateUpdate();
            }
            if (_isCompleted)
            {
                OnStateExit();
            }
        }

        public abstract IState GetNextState();

        protected void Complete()
        {
            _isCompleted = true;
        }

        protected virtual void OnStateEnter() { }
        protected virtual void OnStateUpdate() { }
        protected virtual void OnStateExit() { }

        public override string ToString()
        {
            return string.Format("<State>{0}", this.GetType().Name);
        }
    }
}
