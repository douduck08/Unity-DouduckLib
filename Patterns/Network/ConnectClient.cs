using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DouduckGame.Util;

namespace DouduckGame.Network {
    public sealed class ConnectClient : IConnectionStatus {

        private int m_id;
        public int Id {
            get {
                return m_id;
            }
        }
        public event ConnectionStatusCallback ErrorCallback;

        private TcpClient m_oClient = null;
        private TcpClient Client {
            get {
                if (m_oClient == null || m_oClient.Client == null) {
                    m_oClient = new TcpClient ();
                    m_oClient.NoDelay = true;
                }
                return m_oClient;
            }
        }
        public Socket CoreSocket {
            get {
                return Client.Client;
            }
        }
        public bool IsActive {
            get {
                return Client.Connected;
            }
        }

        private string m_sIP;
        private int m_iPort;
        private float m_fLastSend;     // from Time.realtimeSinceStartup
        private float m_fLastReceive;  // from Time.realtimeSinceStartup

        private ConnectionStatusCallback m_connectCallback = null;
        private ReceiveCallback m_receiveCallback = null;
        private byte[] m_aucReadBuffer = new byte[NetworkUtil.BUFFER_SIZE];

        public ConnectClient (int id) {
            m_id = id;
            m_fLastSend = Time.realtimeSinceStartup;
            m_fLastReceive = Time.realtimeSinceStartup;
        }

        public void Connect (string ip, int port, ConnectionStatusCallback connectCallback, ReceiveCallback receiveCallback) {
            if (IsActive) {
                UnityConsole.LogError ("[ConnectClient] Connection is already active.");
                return;
            }

            m_sIP = ip;
            m_iPort = port;
            m_connectCallback = connectCallback;
            m_receiveCallback = receiveCallback;
            try {
                UnityConsole.Log (string.Format ("[ConnectClient] Try to connect IP: {0}, Port: {1}", ip, port));
                Client.BeginConnect (ip, port, new System.AsyncCallback (EndConnect), null);
            } catch (SocketException ex) {
                UnityConsole.LogError ("[ConnectClient] Connect failed: " + ex.ToString ());
                SendErrorCallback (NetworkStatus.ConnectFailure);
            }
        }

        public void Accept (TcpClient oNewClient) {
            if (IsActive) {
                UnityConsole.LogError ("[ConnectClient] Connection is already active.");
                return;
            }

            if (oNewClient.Connected) {
                m_oClient = oNewClient;
                m_oClient.NoDelay = true;
            } else {
                UnityConsole.LogError ("[ConnectClient] Accept failed, NewClient was closed.");
                SendErrorCallback (NetworkStatus.AcceptFailure);
            }
        }

        public void Receive (ReceiveCallback receiveCallback) {
            if (!IsActive) {
                UnityConsole.LogError ("[Connector] Receive failed, TcpClient was closed.");
                SendErrorCallback (NetworkStatus.ReceiveFailure);
                return;
            }

            m_receiveCallback = receiveCallback;
            try {
                Client.GetStream ().BeginRead (m_aucReadBuffer, 0, NetworkUtil.BUFFER_SIZE, new System.AsyncCallback (EndReceive), null);
            } catch (System.Exception ex) {
                UnityConsole.LogError ("[Connector Error] Recieve failed, " + ex.ToString ());
                SendErrorCallback (NetworkStatus.ReceiveFailure);
            }
        }

        public void Send (byte[] aucPacket) {
            if (!IsActive) {
                UnityConsole.LogError ("[Connector] Send Fail, TcpClient was closed.");
                SendErrorCallback (NetworkStatus.SendFailure);
                return;
            }

            try {
                Client.GetStream ().BeginWrite (aucPacket, 0, aucPacket.Length, new System.AsyncCallback (EndSend), null);
            } catch (System.IO.IOException ex) {
                UnityConsole.LogError ("[Connector Error] Send() Fail, " + ex.ToString ());
                SendErrorCallback (NetworkStatus.SendFailure);
            }
        }

        public void Disconnect () {
            if (m_oClient != null) {
                m_oClient.Close ();
                m_oClient = null;
            }
        }

        private void SendErrorCallback (NetworkStatus error) {
            if (ErrorCallback != null) {
                ErrorCallback (m_id, error);
            }
        }

        // *** AsyncCallback ***
        private void EndConnect (System.IAsyncResult asyn) {
            try {
                Client.EndConnect (asyn);
                if (IsActive) {
                    m_connectCallback (Id, NetworkStatus.ConnectSuccess);
                    m_fLastReceive = Time.realtimeSinceStartup;
                    Receive (m_receiveCallback);
                } else {
                    m_connectCallback (Id, NetworkStatus.ConnectFailure);
                }
            } catch (System.Exception ex) {
                UnityConsole.LogError ("[Connector Error] EndConnect Fail, " + ex.ToString ());
                m_connectCallback (Id, NetworkStatus.ConnectFailure);
            }
        }

        private void EndSend (System.IAsyncResult asyn) {
            try {
                NetworkStream oStream_ = Client.GetStream ();
                oStream_.EndWrite (asyn);
                m_fLastSend = Time.realtimeSinceStartup;
            } catch (System.Exception ex) {
                UnityConsole.LogError ("[Connector Error] EndSend() Fail, " + ex.ToString ());
                SendErrorCallback (NetworkStatus.SendFailure);
            }
        }

        private void EndReceive (System.IAsyncResult asyn) {
            try {
                NetworkStream oStream = Client.GetStream ();
                int iBytesRead_ = oStream.EndRead (asyn);
                if (iBytesRead_ < 1) {
                    Disconnect ();
                    return;
                }

                byte[] newPacket_ = new byte[NetworkUtil.BUFFER_SIZE];
                m_aucReadBuffer.CopyTo (newPacket_, 0);
                m_receiveCallback (Id, newPacket_);
                m_fLastReceive = Time.realtimeSinceStartup;
                Receive (m_receiveCallback);
            } catch (System.Exception ex) {
                UnityConsole.LogError ("[Client Error] EndReceive() Fail, " + ex.ToString ());
                SendErrorCallback (NetworkStatus.ReceiveFailure);
            }
        }
    }
}
