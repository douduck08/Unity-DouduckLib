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
        private ReceiveDelegate m_ReceiveCallback = null;

        // *** Connector ***
        public void AddConnector (uint index, IConnector oConnector) {
            Connectors.Add (index, oConnector);
        }

        public void SetReceiveCallback (ReceiveDelegate dReceiveCallback) {
            m_ReceiveCallback = dReceiveCallback;
        }

        public IConnector GetConnector (uint index) {
            if (Connectors.ContainsKey (index)) {
                return Connectors[index];
            } else {
                return null;
            }
        }

        public void AcceptConnection (uint index, IConnector oConnector, TcpClient oClient) {
            AddConnector (index, oConnector);
            oConnector.Accept (oClient, m_ReceiveCallback);
        }

        public void NewConnection (uint index, IConnector oConnector, string sIP, int iPort) {
            AddConnector (index, oConnector);
            oConnector.Connect (sIP, iPort, m_ReceiveCallback);
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
