using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace DouduckGame.Network {
    public static class IPAddressTool {

        public static IPAddress GetHostIP (string hostname, AddressFamily addressFmily = AddressFamily.InterNetwork) {
            try {
                IPHostEntry host = Dns.GetHostEntry (hostname);
                foreach (IPAddress ip in host.AddressList) {
                    if (ip.AddressFamily == addressFmily) {
                        return ip;
                    }
                }
            } catch (SocketException ex) {
                Util.UnityConsole.LogError ("GetHostIP error: " + ex);
            }
            return null;
        }

        public static IPAddress GetLocalIP (AddressFamily addressFmily = AddressFamily.InterNetwork) {
            try {
                IPHostEntry host = Dns.GetHostEntry (Dns.GetHostName ());
                foreach (IPAddress ip in host.AddressList) {
                    if (ip.AddressFamily == addressFmily) {
                        return ip;
                    }
                }
            } catch (SocketException ex) {
                Util.UnityConsole.LogError ("GetLocalIP error: : " + ex);
            }
            return null;
        }

        public static IPAddress GetIPMask (IPAddress host) {
            try {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces ();
                foreach (NetworkInterface networkInterface in interfaces) {
                    UnicastIPAddressInformationCollection IPInfoCollection = networkInterface.GetIPProperties ().UnicastAddresses;
                    foreach (UnicastIPAddressInformation IPInfo in IPInfoCollection) {
                        if (IPInfo.Address.Equals (host)) {
                            return IPInfo.IPv4Mask;
                        }
                    }
                }
            } catch (SocketException ex) {
                Util.UnityConsole.LogError ("GetIPMask error: : " + ex);
            }
            return null;
        }

        public static IPAddress GetBroadcastIP (IPAddress host, IPAddress mask) {
            byte[] hostBytes = host.GetAddressBytes ();
            byte[] maskBytes = mask.GetAddressBytes ();

            if (hostBytes.Length != maskBytes.Length)
                return null;

            byte[] broadcastIPBytes = new byte[hostBytes.Length];
            for (int i = 0; i < hostBytes.Length; i++) {
                broadcastIPBytes[i] = (byte)(hostBytes[i] | (byte)~maskBytes[i]);
            }
            return new IPAddress (broadcastIPBytes);
        }
    }
}
