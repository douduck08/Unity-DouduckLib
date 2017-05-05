using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using DouduckGame.Util;

namespace DouduckGame.Network {
    public class ConnectionHandler {

        private List<ConnectClient> m_Connectors = new List<ConnectClient> ();
        private ConnectionStatusCallback m_ErrorCallback = null;
        private ReceiveCallback m_ReceiveCallback = null;

        // *** Connector ***
        public void AddConnector (ConnectClient oConnector) {
            m_Connectors.Add (oConnector);
        }

        public void SetCallback (ReceiveCallback receiveCallback, ConnectionStatusCallback errorCallback) {
            m_ReceiveCallback = receiveCallback;
            m_ErrorCallback = errorCallback;
        }

        public ConnectClient GetConnector (int id) {
            int idx_ = m_Connectors.FindIndex (p => p.Id == id);
            if (idx_ != -1) {
                return m_Connectors[idx_];
            } else {
                return null;
            }
        }

        public void AcceptConnection (int id, TcpClient oClient) {
            ConnectClient connector_ = new ConnectClient (id);
            if (m_ErrorCallback != null) {
                connector_.ErrorCallback += m_ErrorCallback;
            }
            AddConnector (connector_);
            connector_.Accept (oClient);
            connector_.Receive(m_ReceiveCallback);
        }

        public void NewConnection (int id, string sIP, int iPort, ConnectionStatusCallback connectCallback) {
            ConnectClient connector_ = new ConnectClient (id);
            if (m_ErrorCallback != null) {
                connector_.ErrorCallback += m_ErrorCallback;
            }
            AddConnector (connector_);
            connector_.Connect (sIP, iPort, connectCallback, m_ReceiveCallback);
        }

        public void RemoveConnection (int id) {
            int idx_ = m_Connectors.FindIndex (p => p.Id == id);
            if (idx_ != -1) {
                m_Connectors[idx_].Disconnect ();
                m_Connectors.RemoveAt (idx_);
            } else {
                UnityConsole.LogError ("[NetworkManager] Remove: No connector index = " + id);
            }
        }

        public void Send (int id, byte[] aucPacket) {
            int idx_ = m_Connectors.FindIndex (p => p.Id == id);
            if (idx_ != -1) {
                m_Connectors[idx_].Send (aucPacket);
            } else {
                UnityConsole.LogError ("[NetworkManager] Send: No connector index = " + id);
            }
        }
    }
}
