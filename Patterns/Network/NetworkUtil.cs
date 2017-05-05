using System.Collections;
using System.Net.Sockets;

namespace DouduckGame.Network {

    public enum NetworkStatus {
        ConnectSuccess,
        ConnectFailure,
        AcceptSuccess,
        AcceptFailure,
        SendSuccess,
        SendFailure,
        ReceiveSuccess,
        ReceiveFailure,
        BroadcastSuccess,
        BroadcastFailure,
        DNSError,
        LoseNetwork
    }

    public delegate void ConnectionStatusCallback (int id, NetworkStatus statusCode);
    public delegate void ListienerCallback (int id, TcpClient oClient);
    public delegate void ReceiveCallback (int id, byte[] aucData);

    public class NetworkUtil {

        public const int BUFFER_SIZE = 1024;

        [System.Obsolete("Use IPAddressTool as altinative tool")]
        public static string LocalIP {
            get {
                return UnityEngine.Network.player.ipAddress;
            }
        }

        public static bool NetworkReachable {
            get {
                return UnityEngine.Application.internetReachability != UnityEngine.NetworkReachability.NotReachable;
            }
        }

        public static bool UsingLocalNetwork {
            get {
                return UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        public static bool UsingMobileMetwork {
            get {
                return UnityEngine.Application.internetReachability != UnityEngine.NetworkReachability.ReachableViaCarrierDataNetwork;
            }
        }
    }
}
