using System.Collections;
using System.Net.Sockets;

namespace DouduckGame.Network {

    public enum NetworkError {
        NONE,
        CONNECT_FAILED,
        RECEIVE_FAILED,
        SEND_FAILED,
        DNS_ERROR
    }

    public delegate void ConnectionErrorCallback (string Name, NetworkError statusCode);
    public delegate void AccepterCallback (TcpClient oClient);
    public delegate void ReceiveCallback (Socket sourceSocket, byte[] aucData);

    public class NetworkUtil {

        public const int BUFFER_SIZE = 1024;

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
