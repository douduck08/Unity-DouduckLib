using UnityEngine;
using System.Collections;
using System.Net.Sockets;

namespace DouduckGame {
	public class AsyncAccepter : IAccepter {

		private AccepterDelegate m_AcceptCallback = null;
		public AsyncAccepter(string sName, string sLocalIP, int iListenPort, AccepterDelegate dAcceptCallback) : base(sName, sLocalIP, iListenPort)
		{
			m_AcceptCallback = dAcceptCallback;
		}

		// private functions: async delegate to accept client
		protected override void WaitForClientConnect() {
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
