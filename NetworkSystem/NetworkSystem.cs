using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DouduckGame;
using DouduckGame.Network;

public enum eConnectorIndex {
	SERVER = 10,
	DISPLAY,
	PLAYER_BASE
}



namespace DouduckGame.Network {
    public class NetworkSystem : IGameSystemMono {

        protected AsyncAccepter m_Accepter;
        protected ConnecterHandler m_ConnecterHandler = new ConnecterHandler ();
        protected PacketHandler m_PacketHandler = new PacketHandler ();

        protected List<AsyncConnector> m_ConnectorBuffer = null;

        public override void StartGameSystem () {
            m_ConnectorBuffer = new List<AsyncConnector> ();
        }

        public override void DestoryGameSystem () {

        }

        void Update () {
            m_PacketHandler.ParseAllPacket ();
        }

        public void SetCallback(ReceiveCallback receiveCallback, ConnectionErrorCallback errorCallback) {
            m_ConnecterHandler.SetCallback(receiveCallback, errorCallback);
        }

        public void SettingPacketHandler (PacketPackager packager, PacketParser[] parser) {
            m_PacketHandler.SetPacketPackager (packager);
            for (int i = 0; i < parser.Length; i++) {
                m_PacketHandler.AddPacketParser(parser[i]);
            }
        }

        // *** Accepter ***
        public void SetAccepter (AsyncAccepter oAccepter) {
            m_Accepter = oAccepter;
        }

        public void StartListen () {
            if (m_Accepter == null) {
                Debug.LogError ("[NetworkSystem] Accepter was not set.");
            } else {
                m_Accepter.StartListen ();
            }
        }

        public void StopListen () {
            if (m_Accepter == null) {
                Debug.LogError ("[NetworkSystem] Accepter was not set.");
            } else {
                m_Accepter.StopListen ();
            }
        }

        // *** Connector ***
        public void AddConnector (uint index, AsyncConnector oConnector) {
            m_ConnecterHandler.AddConnector (index, oConnector);
        }

        public AsyncConnector GetConnector (uint index) {
            return m_ConnecterHandler.GetConnector (index);
        }

        public void AcceptConnection (uint index, string sName, TcpClient oClient) {
            m_ConnecterHandler.AcceptConnection (index, sName, oClient);
        }

        public void NewConnection (uint index, string sName, string sIP, int iPort, ConnectionErrorCallback connectCallback) {
            m_ConnecterHandler.NewConnection(index, sName, sIP, iPort, connectCallback);
        }

        public void RemoveConnection (uint index) {
            m_ConnecterHandler.RemoveConnection (index);
        }

        public void Send (uint index, byte[] aucPacket) {
            m_ConnecterHandler.Send (index, aucPacket);
        }
    }
}