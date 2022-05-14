using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public interface IStateController {
        StateBase currentState { get; }
        void SetState (StateBase state);
        void StateUpdate ();
    }

    public abstract class StateBase {
        IStateController _controller;
        bool _isStarted = false;
        bool _isCompleted = false;

        public IStateController controller { get => _controller; }
        public bool isStarted { get => isStarted; }
        public bool isCompleted { get => _isCompleted; }

        internal void Reset (IStateController controller) {
            _controller = controller;
            _isStarted = false;
            _isCompleted = false;
        }

        internal void StateUpdate () {
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

        internal abstract StateBase GetNextState ();

        internal virtual void OnStateEnter () { }
        internal virtual void OnStateUpdate () { }
        internal virtual void OnStateExit () { }

        public override string ToString () {
            return string.Format ("<State>{0}", this.GetType ().Name);
        }
    }
}