using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class EventDrivenState : StateBase
    {
        public event Action<EventDrivenState> OnStateEnter;
        public event Action<EventDrivenState> OnStateUpdate;
        public event Action<EventDrivenState> OnStateExit;

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

        protected override void OnEnter()
        {
            OnStateEnter?.Invoke(this);
        }

        protected override void OnUpdate()
        {
            OnStateUpdate?.Invoke(this);
        }

        protected override void OnExit()
        {
            OnStateExit?.Invoke(this);
        }
    }
}
