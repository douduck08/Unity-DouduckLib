using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace DouduckGame.Network {
	public class AsyncAccepter {
        public string Name;
        protected IPAddress m_LocalIPAddress;
        protected TcpListener m_oListener = null;
        protected TcpListener Listener {
            get {
                if (m_oListener == null) {
                    Debug.LogError("[Accepter] Listener is null");
                }
                return m_oListener;
            }
        }
        protected bool m_bActive;
        public bool IsActive {
            get {
                return m_bActive;
            }
        }

        private AccepterCallback m_AcceptCallback = null;


        public AsyncAccepter(string sName, string sLocalIP, int iListenPort, AccepterCallback dAcceptCallback) {
            m_LocalIPAddress = IPAddress.Parse(sLocalIP);
            m_oListener = new TcpListener(m_LocalIPAddress, iListenPort);

            Listener.ExclusiveAddressUse = false;
            m_bActive = false;
            Debug.Log("[Accepter] " + Name + ": Ready to work...(" + sLocalIP + ":" + iListenPort + ")");

            m_AcceptCallback = dAcceptCallback;
        }

        public void StartListen() {
            if (!IsActive) {
                Listener.Start();
                m_bActive = true;
                Debug.Log("[Accepter] " + Name + ": Start to listen");
                WaitForClientConnect();
            }
        }

        public void StopListen() {
            if (IsActive) {
                Listener.Stop();
                m_bActive = false;
                Debug.Log("[Accepter] " + Name + ": Stop listening");
            }
        }

        protected void WaitForClientConnect() {
			Listener.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnect), null);
		}

		private void OnClientConnect(System.IAsyncResult asyn) {
			try {
				TcpClient oClient_ = Listener.EndAcceptTcpClient(asyn);
				if (m_AcceptCallback != null) {
					m_AcceptCallback(oClient_);
				}
				WaitForClientConnect();
			}
			catch (SocketException ex) {
				Debug.LogError("[Accepter] " + Name + ": Socket error: " + ex.ToString());
			}
			catch (System.ObjectDisposedException) {
				Debug.Log("[Accepter] " + Name + ": Stopped, listen canceled.");
			}
		}
	}
}
