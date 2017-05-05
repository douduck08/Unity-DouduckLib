using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using DouduckGame.Util;

namespace DouduckGame.Network {
	public class ConnectListener : IConnectionStatus {

        private int m_id;
        public int Id {
            get {
                return m_id;
            }
        }
        public event ConnectionStatusCallback ErrorCallback;

        protected TcpListener m_listener = null;
        protected bool m_bActive;
        public bool IsActive {
            get {
                return m_bActive;
            }
        }

        private int m_port;
        private ListienerCallback m_listienerCallback = null;

        public ConnectListener (IPAddress ip, int port, ListienerCallback listienerCallback) {
            m_listener = new TcpListener(ip, port);
            m_listener.ExclusiveAddressUse = false;
            m_port = port;
            m_bActive = false;
            m_listienerCallback = listienerCallback;
        }

        public void StartListen() {
            if (!IsActive) {
                UnityConsole.Log("[ConnectListener] Start listening at port:" + m_port);
                m_listener.Start();
                m_bActive = true;
                WaitForClientConnect();
            }
        }

        public void StopListen() {
            if (IsActive) {
                UnityConsole.Log("[ConnectListener] Stop listening at port:" + m_port);
                m_listener.Stop();
                m_bActive = false;
            }
        }

        protected void WaitForClientConnect() {
            m_listener.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnect), null);
		}

        private void OnClientConnect (System.IAsyncResult asyn) {
            try {
                TcpClient newClient_ = m_listener.EndAcceptTcpClient (asyn);
                if (m_listienerCallback != null) {
                    m_listienerCallback (Id, newClient_);
                }
                WaitForClientConnect ();
            } catch (SocketException ex) {
                UnityConsole.LogError ("[ConnectListener] Socket error: " + ex.ToString ());
                SendErrorCallback (NetworkStatus.AcceptFailure);
            } catch (System.ObjectDisposedException) {
                UnityConsole.Log ("[ConnectListener] Listener stopped.");
            }
        }

        private void SendErrorCallback (NetworkStatus error) {
            if (ErrorCallback != null) {
                ErrorCallback (m_id, error);
            }
        }
    }
}
