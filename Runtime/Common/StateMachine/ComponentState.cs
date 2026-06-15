using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public abstract class ComponentState : MonoBehaviour, IState
    {
        public IStateController Controller => _controller;
        public bool IsStarted => _isStarted;
        public bool IsCompleted => _isCompleted;

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
                OnEnter();
            }
            if (!_isCompleted)
            {
                OnUpdate();
            }
            if (_isCompleted)
            {
                OnExit();
            }
        }

        public abstract IState GetNextState();

        protected void Complete()
        {
            _isCompleted = true;
        }

        protected virtual void OnEnter() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnExit() { }
    }
}
