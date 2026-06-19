using System;

namespace DouduckLib
{
    public class State : IState
    {
        public IStateController Controller => _controller;
        public bool IsStarted => _isStarted;
        public bool IsCompleted => _isCompleted;

        public event Action<State> OnStateEnter;
        public event Action<State> OnStateUpdate;
        public event Action<State> OnStateExit;

        IStateController _controller;
        bool _isStarted = false;
        bool _isCompleted = false;
        Func<State, IState> _getNextState;

        public void Initialize(IStateController controller)
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

        public virtual IState GetNextState()
        {
            return _getNextState?.Invoke(this);
        }

        public State SetNextStateFunction(Func<State, IState> func)
        {
            _getNextState = func;
            return this;
        }

        public void SetComplete()
        {
            Complete();
        }

        protected void Complete()
        {
            _isCompleted = true;
        }

        protected virtual void OnEnter()
        {
            OnStateEnter?.Invoke(this);
        }

        protected virtual void OnUpdate()
        {
            OnStateUpdate?.Invoke(this);
        }

        protected virtual void OnExit()
        {
            OnStateExit?.Invoke(this);
        }

        public override string ToString()
        {
            return string.Format("<State>{0}", GetType().Name);
        }
    }
}
