using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public class EventDrivenState : StateBase {
        public event Action<EventDrivenState> onStateEnter;
        public event Action<EventDrivenState> onStateUpdate;
        public event Action<EventDrivenState> onStateExit;
        public event Func<EventDrivenState, EventDrivenState> getNextState;

        public override void OnStateEnter () {
            onStateEnter?.Invoke (this);
        }

        public override void OnStateUpdate () {
            onStateUpdate?.Invoke (this);
        }

        public override void OnStateExit () {
            onStateExit?.Invoke (this);
        }

        public override IState GetNextState () {
            return getNextState?.Invoke (this);
        }

        public void SetComplete () {
            Complete ();
        }
    }
}