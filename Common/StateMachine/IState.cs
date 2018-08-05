namespace DouduckLib {
    public interface IState {
        IStateController stateController { get; }
        bool isStarted { get; }
        bool isCompleted { get; }
        void StateUpdate ();
        IState GetNextState ();
    }
}