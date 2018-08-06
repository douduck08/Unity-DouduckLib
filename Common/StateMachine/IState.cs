namespace DouduckLib {
    public interface IState {
        IStateController controller { get; set; }
        bool isStarted { get; }
        bool isCompleted { get; }
        void StateUpdate ();
        IState GetNextState ();
    }
}