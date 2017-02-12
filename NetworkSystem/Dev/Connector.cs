using System.Collections;
using System.Net.Sockets;
using UnityEngine;

namespace DouduckGame.Network.Dev {
    public abstract class Connector {
        public string Name;
        protected TcpClient m_oClient = null;
        protected TcpClient Client {
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
        public bool IsAlive {
            get {
                return Client.Connected;
            }
        }

        private byte[] m_aucReadBuffer = null;
        protected byte[] ReadBuffer {
            get {
                if (m_aucReadBuffer == null) {
                    m_aucReadBuffer = new byte[NetworkUtil.BUFFER_SIZE];
                }
                return m_aucReadBuffer;
            }
        }

        protected ConnectionErrorCallback m_ConnectStatusCallback = null;
        protected ConnectionErrorCallback m_RecieveStatusCallback = null;
        protected ConnectionErrorCallback m_SendStatusCallback = null;

        public Connector (string sName) {
            Name = sName;
        }

        public void Connect (string sIP, int iPort, ConnectionErrorCallback statusCallback) {
            if (IsAlive) {
                Debug.LogWarning ("[Connector] " + Name + ": Connection is already alive.");
                return;
            }
            m_ConnectStatusCallback = statusCallback;
            DoConnect (sIP, iPort);
        }

        public void Accept (TcpClient oNewClient) {
            if (IsAlive) {
                Debug.LogWarning ("[Connector] " + Name + ": Connection is already alive.");
                return;
            }
            DoAccept (oNewClient);
        }

        public void Disconnect () {
            if (IsAlive) {
                Debug.Log ("[Connector] " + Name + ": Close client.");
                DoDisconnect ();
                m_oClient = null;
            }
        }

        public void Receive (ReceiveCallback receiveCallback, ConnectionErrorCallback statusCallback) {
            if (!IsAlive) {
                Debug.LogError ("[Connector] " + Name + ": Receive() Fail, TcpClient was closed.");
                return;
            }
            m_RecieveStatusCallback = statusCallback;
            DoReceive (receiveCallback);
        }

        public void Send (byte[] aucPacket, ConnectionErrorCallback statusCallback) {
            if (!IsAlive) {
                Debug.LogError ("[Connector] " + Name + ": Send() Fail, TcpClient was closed.");
                return;
            }
            m_SendStatusCallback = statusCallback;
            DoSend (aucPacket);
        }

        protected abstract void DoConnect (string sIP, int iPort);
        protected abstract void DoAccept (TcpClient oNewClient);
        protected abstract void DoDisconnect ();
        protected abstract void DoReceive (ReceiveCallback receiveCallback);
        protected abstract void DoSend (byte[] aucPacket);

        protected void NotifyConnectResult(bool success = true) {
            if (m_ConnectStatusCallback == null) return;
            if (success) {
                //m_ConnectStatusCallback (Name, NetworkError.CONNECT_SUCCESS);
            } else {
                //m_ConnectStatusCallback (Name, NetworkError.CONNECT_FAILED);
            }
        }

        protected void NotifyReceiveResult (bool success = true) {
            if (m_ConnectStatusCallback == null) return;
            if (success) {
                //m_ConnectStatusCallback (Name, NetworkError.RECEIVE_SUCCESS);
            } else {
                //m_ConnectStatusCallback (Name, NetworkError.RECEIVE_FAILED);
            }
        }

        protected void NotifySendResult (bool success = true) {
            if (m_ConnectStatusCallback == null) return;
            if (success) {
                //m_ConnectStatusCallback (Name, NetworkError.SEND_SUCCESS);
            } else {
                //m_ConnectStatusCallback (Name, NetworkError.SEND_FAILED);
            }
        }
    }
}
