using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace DouduckGame.Network {
    public class ConnecterHandler {

        private Dictionary<uint, AsyncConnector> m_Connectors = null;
        private Dictionary<uint, AsyncConnector> Connectors {
            get {
                if (m_Connectors == null) {
                    m_Connectors = new Dictionary<uint, AsyncConnector> ();
                }
                return m_Connectors;
            }
        }

        private ConnectionErrorCallback m_ErrorCallback = null;
        private ReceiveCallback m_ReceiveCallback = null;

        // *** Connector ***
        public void AddConnector (uint index, AsyncConnector oConnector) {
            Connectors.Add (index, oConnector);
        }

        public void SetCallback (ReceiveCallback receiveCallback, ConnectionErrorCallback errorCallback) {
            m_ReceiveCallback = receiveCallback;
            m_ErrorCallback = errorCallback;
        }

        public AsyncConnector GetConnector (uint index) {
            if (Connectors.ContainsKey (index)) {
                return Connectors[index];
            } else {
                return null;
            }
        }

        public void AcceptConnection (uint index, string sName, TcpClient oClient) {
            AsyncConnector connector_ = new AsyncConnector(sName, m_ReceiveCallback, m_ErrorCallback);
            AddConnector (index, connector_);
            connector_.Accept (oClient);
            connector_.Receive(m_ReceiveCallback);
        }

        public void NewConnection (uint index, string sName, string sIP, int iPort, ConnectionErrorCallback connectCallback) {
            AsyncConnector connector_ = new AsyncConnector(sName, m_ReceiveCallback, m_ErrorCallback);
            AddConnector(index, connector_);
            connector_.Connect (sIP, iPort, connectCallback);
        }

        public void RemoveConnection (uint index) {
            if (Connectors.ContainsKey (index)) {
                Connectors[index].Disconnect ();
                Connectors.Remove (index);
            } else {
                Debug.LogError ("[NetworkManager] Remove: No connector index = " + index);
            }
        }

        public void Send (uint index, byte[] aucPacket) {
            if (Connectors.ContainsKey (index)) {
                Connectors[index].Send (aucPacket);
            } else {
                Debug.LogError ("[NetworkManager] Send: No connector index = " + index);
            }
        }
    }
}
