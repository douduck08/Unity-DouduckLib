namespace DouduckLib {
    public interface IUpdatable {
        void OnUpdate ();
    }

    public interface IFixedUpdatable {
        void OnFixedUpdate ();
    }

    public interface ILateUpdatable {
        void OnLateUpdate ();
    }
}