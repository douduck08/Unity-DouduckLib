using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using DouduckGame.Util;

namespace DouduckGame.Network {
    public class BroadcastClient : IConnectionStatus {

        private int m_id;
        public int Id {
            get {
                return m_id;
            }
        }
        public event ConnectionStatusCallback ErrorCallback;

        private string m_sIP;
        private int m_iPort;
        private UdpClient m_UdpClient;
        private IPAddress m_broadcastIP;
        private IPEndPoint m_EndPoint;
        private ReceiveCallback m_receiveCallback;

        public bool IsActive {
            get {
                return m_broadcastIP != null;
            }
        }

        public BroadcastClient (int id, int port, ReceiveCallback receiveCallback) {
            m_id = id;
            m_iPort = port;
            m_receiveCallback = receiveCallback;

            try {
                IPAddress ip = IPAddressTool.GetLocalIP ();
                IPAddress mask = IPAddressTool.GetIPMask (ip);
                m_broadcastIP = IPAddressTool.GetBroadcastIP (ip, mask);
                m_sIP = m_broadcastIP.ToString ();
            } catch (System.Exception ex) {
                UnityConsole.LogError (ex);
            }

            m_EndPoint = new IPEndPoint (IPAddress.Any, port);
            m_UdpClient = new UdpClient (m_EndPoint);
            Receive (m_receiveCallback);
        }

        public void Broadcast (byte[] aucPacket) {
            if (!IsActive) {
                UnityConsole.LogError ("[BroadcastClient] Broadcast Fail, find no broadcast IP.");
                SendErrorCallback (NetworkStatus.BroadcastFailure);
                return;
            }

            try {
                m_UdpClient.BeginSend (aucPacket, aucPacket.Length, m_sIP, m_iPort, new System.AsyncCallback (EndSend), null);
            } catch (System.Exception ex) {
                UnityConsole.LogError ("[BroadcastClient] Broadcast failed: " + ex.ToString ());
                SendErrorCallback (NetworkStatus.BroadcastFailure);
            }
        }

        private void EndSend (System.IAsyncResult asyn) {
            try {
                m_UdpClient.EndSend (asyn);
            } catch (System.Exception ex) {
                UnityConsole.Log ("[BroadcastClient] EndSend() Fail, " + ex.ToString ());
                SendErrorCallback (NetworkStatus.BroadcastFailure);
            }
        }

        public void Receive (ReceiveCallback receiveCallback) {
            m_receiveCallback = receiveCallback;
            try {
                m_UdpClient.BeginReceive (new System.AsyncCallback (EndReceive), null);
            } catch (System.Exception ex) {
                UnityConsole.LogError ("[BroadcastClient] Recieve failed: " + ex.ToString ());
                SendErrorCallback (NetworkStatus.ReceiveFailure);
            }
        }

        private void EndReceive (System.IAsyncResult asyn) {
            try {
                byte[] aucReadBuffer = m_UdpClient.EndReceive (asyn, ref m_EndPoint);
                m_receiveCallback (m_id, aucReadBuffer);
                Receive (m_receiveCallback);
            } catch (System.Exception ex) {
                UnityConsole.LogError ("[BroadcastClient] EndReceive failed: " + ex.ToString ());
                SendErrorCallback (NetworkStatus.ReceiveFailure);
            }
        }

        private void SendErrorCallback (NetworkStatus error) {
            if (ErrorCallback != null) {
                ErrorCallback (m_id, error);
            }
        }
    }
}