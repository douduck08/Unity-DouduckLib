using UnityEngine;
using System.Collections;
using System.Net.Sockets;

namespace DouduckGame {
	public class AsyncConnector : IConnector {

		private string m_sIP;
		private int m_iPort;
		private float m_fLastReceive;  // use Time.realtimeSinceStartup
		private ReceiveDelegate m_ReceiveCallback = null;
		public AsyncConnector(string sName, ConnectDelegate dConnectCallback) : base(sName, dConnectCallback) {}

		// *** Public method ***
		public override void Accept(TcpClient oNewClient, ReceiveDelegate dReceiveCallback) {
			if (oNewClient.Connected) {
				Debug.Log("[Connector] " + Name + ": Accept new client, start Receive Callback");
				m_oClient = oNewClient;
				m_oClient.NoDelay = true;
				m_ReceiveCallback = dReceiveCallback;
				Receive(m_ReceiveCallback);
			} else {
				Debug.LogError("[Connector] " + Name + ": Accept() Fail, NewClient was closed.");
			}
		}

		public override void Connect(string sIP, int iPort, ReceiveDelegate dReceiveCallback) {
			if (IsAlive) {
				Debug.LogWarning("[Connector] " + Name + ": Connection is already alive.");
				return;
			}
			try{
				m_sIP = sIP;
				m_iPort = iPort;
				m_ReceiveCallback = dReceiveCallback;
				Debug.Log("[Connector] " + Name + ": Try to connect (IP: " + sIP + ")");
				Client.BeginConnect(sIP, iPort, new System.AsyncCallback(EndConnect), null);
			} catch (SocketException ex) {
				Debug.LogError("[Connector] " + Name + ": Connect() Fail, " + ex.ToString());
				Disconnect();
				return;
			}
		}

		public override void Reconnect() {
			Connect(m_sIP, m_iPort, m_ReceiveCallback);
		}

		public override void Disconnect() {
			if (IsAlive) {
				Debug.Log("[Connector] " + Name + ": Close client.");
				Client.Close();
				m_oClient = null;
			}
		}

		public override void Send(byte[] aucPacket) {
			if (!IsAlive) {
				Debug.LogError("[Connector] " + Name + ": Send() Fail, TcpClient was closed.");
				return;
			}
			try {
				Client.GetStream().BeginWrite(aucPacket, 0, aucPacket.Length, new System.AsyncCallback(EndSend), null);
			} catch (System.IO.IOException ex) {
				Debug.LogError("[Connector Error] " + Name + ": Send() Fail, " + ex.ToString());
				Disconnect();
			}
		}

		public override void Receive(ReceiveDelegate dReceiveCallback) {
			m_ReceiveCallback = dReceiveCallback;
			try {
				Client.GetStream().BeginRead(ReadBuffer, 0, BUFFER_SIZE, new System.AsyncCallback(EndReceive), null);
			}
			catch (System.Exception ex) {
				Debug.LogError("[Connector Error] " + Name + ": Recieve() Fail, " + ex.ToString());
			}
		}

		// *** AsyncCallback ***
		private void EndConnect(System.IAsyncResult asyn) {
			try {
				Client.EndConnect(asyn);
				if (IsAlive) {
					Debug.Log("[Connector] " + Name + ": Connect Success, start Receive Callback");
					m_ConnectCallback();
					Receive(m_ReceiveCallback);
				}
			}
			catch (System.Exception ex) {
				Debug.LogError("[Connector Error] " + Name + ": EndConnect() Fail, " + ex.ToString());
			}
		}

		private void EndSend(System.IAsyncResult asyn) {
			try {
				NetworkStream oStream_ = Client.GetStream();
				oStream_.EndWrite(asyn);
			}
			catch (System.Exception ex) {
				Debug.LogError("[Connector Error] " + Name + ": EndSend() Fail, " + ex.ToString());
				Disconnect();
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
				}
				Receive(m_ReceiveCallback);
			}
			catch (System.Exception ex) {
				Debug.LogError("[Client Error] " + Name + ": EndReceive() Fail, " + ex.ToString());
				Disconnect();
			}
		}
	}
}
