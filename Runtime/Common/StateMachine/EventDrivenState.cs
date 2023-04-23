using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class EventDrivenState : StateBase
    {
        public event Action<EventDrivenState> onStateEnter;
        public event Action<EventDrivenState> onStateUpdate;
        public event Action<EventDrivenState> onStateExit;

        Func<EventDrivenState, EventDrivenState> _getNextState;

        public EventDrivenState SetNextStateFunction(Func<EventDrivenState, EventDrivenState> func)
        {
            _getNextState = func;
            return this;
        }

        public override IState GetNextState()
        {
            return _getNextState?.Invoke(this);
        }

        public void SetComplete()
        {
            Complete();
        }

        protected override void OnStateEnter()
        {
            onStateEnter?.Invoke(this);
        }

        protected override void OnStateUpdate()
        {
            onStateUpdate?.Invoke(this);
        }

        protected override void OnStateExit()
        {
            onStateExit?.Invoke(this);
        }
    }
}
