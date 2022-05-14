using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public class EventDrivenState : StateBase {
        public event Action<EventDrivenState> onStateEnter;
        public event Action<EventDrivenState> onStateUpdate;
        public event Action<EventDrivenState> onStateExit;

        Func<EventDrivenState, EventDrivenState> _getNextState;
        public Func<EventDrivenState, EventDrivenState> getNextState { set => _getNextState = value; }

        internal override void OnStateEnter () {
            onStateEnter?.Invoke (this);
        }

        internal override void OnStateUpdate () {
            onStateUpdate?.Invoke (this);
        }

        internal override void OnStateExit () {
            onStateExit?.Invoke (this);
        }

        internal override StateBase GetNextState () {
            return _getNextState?.Invoke (this);
        }

        public void SetComplete () {
            Complete ();
        }
    }
}