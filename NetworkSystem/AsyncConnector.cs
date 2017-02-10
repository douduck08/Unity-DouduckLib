using UnityEngine;
using System.Collections;
using System.Net.Sockets;

namespace DouduckGame.Network {
	public sealed class AsyncConnector {
        public string Name;
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
        public bool IsAlive {
            get {
                return Client.Connected;
            }
        }

        private byte[] m_aucReadBuffer = null;
        private byte[] ReadBuffer {
            get {
                if (m_aucReadBuffer == null) {
                    m_aucReadBuffer = new byte[NetworkUtil.BUFFER_SIZE];
                }
                return m_aucReadBuffer;
            }
        }

        private string m_sIP;
		private int m_iPort;
		private float m_fLastReceive;  // from Time.realtimeSinceStartup

        private ConnectionErrorCallback m_ErrorCallback = null;
        private ReceiveCallback m_ReceiveCallback = null;

		public AsyncConnector(string sName, ConnectionErrorCallback errorCallback) {
            Name = sName;
            m_ErrorCallback = errorCallback;
        }

        public AsyncConnector (string sName, ReceiveCallback receiveCallback, ConnectionErrorCallback errorCallback) {
            Name = sName;
            m_ReceiveCallback = receiveCallback;
            m_ErrorCallback = errorCallback;
        }

        public void Connect (string sIP, int iPort, System.Action connectCallback) {
            if (IsAlive) {
                Debug.LogWarning ("[Connector] " + Name + ": Connection is already alive.");
                return;
            }

            try {
                m_sIP = sIP;
                m_iPort = iPort;
                Debug.Log ("[Connector] " + Name + ": Try to connect (IP: " + sIP + ")");
                Client.BeginConnect (sIP, iPort, new System.AsyncCallback (EndConnect), connectCallback);
            } catch (SocketException ex) {
                Debug.LogError ("[Connector] " + Name + ": Connect() Fail, " + ex.ToString ());
                ErrorNotify (NetworkError.CONNECT_FAILED);
                Disconnect ();
                return;
            }
        }

        public void Accept (TcpClient oNewClient) {
            if (IsAlive) {
                Debug.LogWarning ("[Connector] " + Name + ": Connection is already alive.");
                return;
            }

            if (oNewClient.Connected) {
                Debug.Log ("[Connector] " + Name + ": Accept new client, start Receive Callback");
                m_oClient = oNewClient;
                m_oClient.NoDelay = true;
            } else {
                Debug.LogError ("[Connector] " + Name + ": Accept() Fail, NewClient was closed.");
            }
        }

        public void Disconnect () {
            if (IsAlive) {
                Debug.Log ("[Connector] " + Name + ": Close client.");
                Client.Close ();
                m_oClient = null;
            }
        }

        public void Receive (ReceiveCallback receiveCallback, ConnectionCallback statusCallback) {
            if (!IsAlive) {
                Debug.LogError ("[Connector] " + Name + ": Receive() Fail, TcpClient was closed.");
                return;
            }
            m_RecieveStatusCallback = statusCallback;
            m_ReceiveCallback = receiveCallback;
            try {
                Client.GetStream ().BeginRead (ReadBuffer, 0, NetworkUtil.BUFFER_SIZE, new System.AsyncCallback (EndReceive), null);
            } catch (System.Exception ex) {
                Debug.LogError ("[Connector Error] " + Name + ": Recieve() Fail, " + ex.ToString ());
            }
        }

        public void Send (byte[] aucPacket, ConnectionCallback statusCallback) {
            if (!IsAlive) {
                Debug.LogError ("[Connector] " + Name + ": Send() Fail, TcpClient was closed.");
                return;
            }
            m_SendStatusCallback = statusCallback;

            if (!IsAlive) {
                Debug.LogError ("[Connector] " + Name + ": Send() Fail, TcpClient was closed.");
                return;
            }
            try {
                Client.GetStream ().BeginWrite (aucPacket, 0, aucPacket.Length, new System.AsyncCallback (EndSend), null);
            } catch (System.IO.IOException ex) {
                Debug.LogError ("[Connector Error] " + Name + ": Send() Fail, " + ex.ToString ());
                Disconnect ();
            }
        }

        public void Reconnect () {
            Connect (m_sIP, m_iPort, m_ConnectStatusCallback);
        }

        private void ErrorNotify(NetworkError error) {
            if (m_ErrorCallback != null) {
                m_ErrorCallback (Name, error);
            }
        }


        // *** AsyncCallback ***
        private void EndConnect(System.IAsyncResult asyn) {
            try {
                Client.EndConnect (asyn);
                if (IsAlive) {
                    Debug.Log ("[Connector] " + Name + ": Connect Success, start Receive Callback");
                    NotifyConnectResult ();
                    Receive (m_ReceiveCallback, m_RecieveStatusCallback);
                } else {
                    NotifyConnectResult (false);
                }
            } catch (System.Exception ex) {
                Debug.LogError ("[Connector Error] " + Name + ": EndConnect() Fail, " + ex.ToString ());
                NotifyConnectResult (false);
            }
        }

		private void EndSend(System.IAsyncResult asyn) {
            try {
                NetworkStream oStream_ = Client.GetStream ();
                oStream_.EndWrite (asyn);
            } catch (System.Exception ex) {
                Debug.LogError ("[Connector Error] " + Name + ": EndSend() Fail, " + ex.ToString ());
                Disconnect ();
            }
		}

		private void EndReceive(System.IAsyncResult asyn) {
			try {
				NetworkStream oStream = Client.GetStream();
				int iBytesRead_;
				iBytesRead_ = oStream.EndRead(asyn);
				if (iBytesRead_ < 1) {
					Debug.Log("[Connector] " + Name + ": NetworkStream is ended.");
					Disconnect();
					return;
				}
				if(CoreSocket != null) {
					m_ReceiveCallback(CoreSocket, ReadBuffer);
				} else {
					Debug.LogError("[Client Error] " + Name + ": null socket");
                    NotifyReceiveResult (false);
                    Disconnect ();
                }
                Receive (m_ReceiveCallback, m_RecieveStatusCallback);
            }
			catch (System.Exception ex) {
				Debug.LogError("[Client Error] " + Name + ": EndReceive() Fail, " + ex.ToString());
                NotifyReceiveResult (false);
                Disconnect ();
			}
		}
	}
}
