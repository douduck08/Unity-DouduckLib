using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public interface IState {
        IStateController controller { get; }
        bool isStarted { get; }
        bool isCompleted { get; }
        void Reset (IStateController controller);
        void StateUpdate ();
        IState GetNextState ();
    }

    public interface IStateController {
        IState currentState { get; }
        void SetState (IState state);
        void StateUpdate ();
    }

    public abstract class StateBase : IState {
        IStateController _controller;
        bool _isStarted = false;
        bool _isCompleted = false;

        public IStateController controller { get => _controller; }
        public bool isStarted { get => isStarted; }
        public bool isCompleted { get => _isCompleted; }

        public void Reset (IStateController controller) {
            _controller = controller;
            _isStarted = false;
            _isCompleted = false;
        }

        public void StateUpdate () {
            if (!_isStarted) {
                _isStarted = true;
                OnStateEnter ();
            }
            if (!_isCompleted) {
                OnStateUpdate ();
            }
            if (_isCompleted) {
                OnStateExit ();
            }
        }

        protected void Complete () {
            _isCompleted = true;
        }

        public abstract IState GetNextState ();

        public virtual void OnStateEnter () { }
        public virtual void OnStateUpdate () { }
        public virtual void OnStateExit () { }

        public override string ToString () {
            return string.Format ("<State>{0}", this.GetType ().Name);
        }
    }
}