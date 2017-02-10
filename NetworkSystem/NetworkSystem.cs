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

        private bool m_bHasError = false;
        public bool HasError {
            get {
                return m_bHasError;
            }
        }
        private eNetworkError m_eErrorCode = eNetworkError.NONE;
        public eNetworkError ErrorCode {
            get {
                return m_eErrorCode;
            }
            protected set {
                m_bHasError = true;
                m_eErrorCode = value;
            }
        }


        protected IAccepter m_Accepter;
        protected ConnecterHandler m_ConnecterHandler = new ConnecterHandler ();
        protected PacketHandler m_PacketHandler = new PacketHandler ();

        protected List<AsyncConnector> m_ConnectorBuffer = null;
        protected static ReceiveDelegate m_ReceiveCallBack = null;

        public override void StartGameSystem () {
            m_ConnectorBuffer = new List<AsyncConnector> ();
            m_ReceiveCallBack = new ReceiveDelegate (m_PacketHandler.NewPacket);
        }

        public override void DestoryGameSystem () {

        }

        void Update () {
            m_PacketHandler.ParseAllPacket ();
        }

        public void SettingPacketHandler (PacketPackager packager, PacketParser parser) {
            m_ConnecterHandler.SetReceiveCallback (m_ReceiveCallBack);
            m_PacketHandler.SetPacketPackager (packager);
            m_PacketHandler.SetPacketParser (parser);
        }

        // *** Accepter ***
        public void SetAccepter (IAccepter oAccepter) {
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
        public void AddConnector (uint index, IConnector oConnector) {
            m_ConnecterHandler.AddConnector (index, oConnector);
        }

        public IConnector GetConnector (uint index) {
            return m_ConnecterHandler.GetConnector (index);
        }

        public void AcceptConnection (uint index, IConnector oConnector, TcpClient oClient) {
            m_ConnecterHandler.AcceptConnection (index, oConnector, oClient);
        }

        public void NewConnection (uint index, IConnector oConnector, string sIP, int iPort) {
            m_ConnecterHandler.NewConnection (index, oConnector, sIP, iPort);
        }

        public void RemoveConnection (uint index) {
            m_ConnecterHandler.RemoveConnection (index);
        }

        public void Send (uint index, byte[] aucPacket) {
            m_ConnecterHandler.Send (index, aucPacket);
        }
    }
}