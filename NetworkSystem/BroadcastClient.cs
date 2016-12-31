using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace DouduckGame {
	public class BroadcastClient : IConnector {

		private string m_sIP;
		private int m_iPort;
		private IPEndPoint m_EndPoint;
		private ReceiveDelegate m_ReceiveCallback = null;

		private UdpClient m_UdpClient;

		public BroadcastClient(string sName) : base(sName, null) {
		}

		public override void Accept(TcpClient oNewClient, ReceiveDelegate dReceiveCallback) {
			Debug.LogWarning ("[BroadcastClient] Not support Accept()");
		}
		public override void Connect (string sIP, int iPort, ReceiveDelegate dReceiveCallback) {
			m_sIP = sIP;
			m_iPort = iPort;
			m_ReceiveCallback = dReceiveCallback;

			m_EndPoint = new IPEndPoint (IPAddress.Any, m_iPort);
			m_UdpClient = new UdpClient (m_iPort);

			Receive(m_ReceiveCallback);
		}
		public override void Reconnect () {
			Debug.LogWarning ("[BroadcastClient] Not support Reconnect()");
		}
		public override void Disconnect () {
			Debug.LogWarning ("[BroadcastClient] Not support Disconnect()");
		}
		public override void Receive (ReceiveDelegate dReceiveCallback) {
			m_ReceiveCallback = dReceiveCallback;
			try {
				m_UdpClient.BeginReceive(new System.AsyncCallback(EndReceive), null);
			}
			catch (System.Exception ex) {
				Debug.LogError("[BroadcastClient Error] " + Name + ": Recieve() Fail, " + ex.ToString());
			}
		}
		public override void Send (byte[] aucPacket) {
			try {
				m_UdpClient.Send(aucPacket, aucPacket.Length, m_sIP, m_iPort);
			}
			catch (System.Exception ex) {
				Debug.LogError("[Connector Error] " + Name + ": Send() Fail, " + ex.ToString());
			}
		}

		private void EndReceive(System.IAsyncResult asyn) {
			try {
				byte[] aucReadBuffer = m_UdpClient.EndReceive(asyn, ref m_EndPoint);
				m_ReceiveCallback(null, aucReadBuffer);

				Receive(m_ReceiveCallback);
			}
			catch (System.Exception ex) {
				Debug.LogError("[Client Error] " + Name + ": EndReceive() Fail, " + ex.ToString());
			}
		}
	}
}