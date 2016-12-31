using UnityEngine;
using System.Collections;
using System.Net.Sockets;

namespace DouduckGame {
	public delegate void ConnectDelegate();
	public delegate void ReceiveDelegate(Socket CoreSocket, byte[] ReadBuffer);
	public abstract class IConnector {

		public string Name;
		protected const int BUFFER_SIZE = 1024;
		protected ConnectDelegate m_ConnectCallback = null;
		protected TcpClient m_oClient = null;
		protected TcpClient Client {
			get {
				if (m_oClient == null || m_oClient.Client == null) {
					m_oClient = new TcpClient();
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
					m_aucReadBuffer = new byte[BUFFER_SIZE];
				}
				return m_aucReadBuffer;
			}
		}

		// *** Public method ***
		public IConnector(string sName, ConnectDelegate dConnectCallback) {
			Name = sName;
			m_ConnectCallback = dConnectCallback;
		}
		public abstract void Accept(TcpClient oNewClient, ReceiveDelegate dReceiveCallback);
		public abstract void Connect (string sIP, int iPort, ReceiveDelegate dReceiveCallback);
		public abstract void Reconnect ();
		public abstract void Disconnect ();
		public abstract void Receive (ReceiveDelegate dReceiveCallback);
		public abstract void Send (byte[] aucPacket);
	}
}
