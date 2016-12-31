using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DouduckGame {
	public class NetworkManager : Singleton<NetworkManager> {

		private IAccepter m_Accepter;
		private Dictionary<uint, IConnector> m_Connectors = null;
		private Dictionary<uint, IConnector> Connectors {
			get {
				if (m_Connectors == null) {
					m_Connectors = new Dictionary<uint, IConnector> ();
				}
				return m_Connectors;
			}
		}
		private ReceiveDelegate m_ReceiveCallback = null;

		// *** Accepter ***
		public void SetAccepter(IAccepter oAccepter) {
			m_Accepter = oAccepter;
		}
		public void StartListen() {
			if (m_Accepter == null) {
				Debug.LogError("[NetworkManager] Accepter was not set.");
			} else {
				m_Accepter.StartListen();
			}
		}
		public void StopListen() {
			if (m_Accepter == null) {
				Debug.LogError("[NetworkManager] Accepter was not set.");
			} else {
				m_Accepter.StopListen();
			}
		}
		// *** Connector ***
		public void AddConnector(uint index, IConnector oConnector) {
			Connectors.Add(index, oConnector);
		}
		public void SetReceiveCallback (ReceiveDelegate dReceiveCallback) {
			m_ReceiveCallback = dReceiveCallback;
		}
		public IConnector GetConnector (uint index) {
			if (Connectors.ContainsKey(index)) {
				return Connectors[index];
			} else {
				Debug.LogError("[NetworkManager] No connector index = " + index);
				return null;
			}
		}
		public void AcceptConnection(uint index, IConnector oConnector, TcpClient oClient) {
			AddConnector(index, oConnector);
			oConnector.Accept(oClient, m_ReceiveCallback);
		}
		public void NewConnection(uint index, IConnector oConnector, string sIP, int iPort) {
			AddConnector(index, oConnector);
			oConnector.Connect(sIP, iPort, m_ReceiveCallback);
		}
		public void RemoveConnection(uint index) {
			if (Connectors.ContainsKey(index)) {
				Connectors[index].Disconnect();
				Connectors.Remove(index);
			} else {
				Debug.LogError("[NetworkManager] Remove: No connector index = " + index);
			}
		}
		public void Send(uint index, byte[] aucPacket) {
			if (Connectors.ContainsKey(index)) {
				Connectors [index].Send(aucPacket);
			} else {
				Debug.LogError("[NetworkManager] Send: No connector index = " + index);
			}
		}
	}
}
