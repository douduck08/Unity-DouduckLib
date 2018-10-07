namespace DouduckLib {
    public interface IStateController {
        IState currentState { get; }
        void SetState (IState state);
        void StateUpdate ();
    }
}