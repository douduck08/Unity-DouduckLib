using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public abstract class State : IState {
        internal IStateController _stateController;
        public IStateController stateController {
            get {
                return _stateController;
            }
        }

        bool _isStarted = false;
        public bool isStarted {
            get {
                return isStarted;
            }
        }

        bool _isCompleted = false;
        public bool isCompleted {
            get {
                return _isCompleted;
            }
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

        public abstract IState GetNextState ();

        public virtual void OnStateEnter () { }
        public virtual void OnStateUpdate () { }
        public virtual void OnStateExit () { }

        public override string ToString () {
            return string.Format ("<State>{0}", this.GetType ().Name);
        }
    }
}