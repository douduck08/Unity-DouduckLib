namespace DouduckGame.Network {
    public interface IConnectionStatus {
        int Id {
            get;
        }
        bool IsActive {
            get;
        }
        event ConnectionStatusCallback ErrorCallback;
    }
}
