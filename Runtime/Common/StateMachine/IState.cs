namespace DouduckLib
{
    public interface IStateController
    {
        IState CurrentState { get; }
        void SetState(IState state);
        void StateUpdate();
    }

    public interface IState
    {
        IStateController Controller { get; }
        bool IsStarted { get; }
        bool IsCompleted { get; }
        void Initialize(IStateController controller);
        void StateUpdate();
        IState GetNextState();
    }
}
