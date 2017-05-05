using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DouduckGame;
using DouduckGame.Network;

namespace DouduckGame.Network {
    public class NetworkModule : IModular, IModuleUpdatable {

        protected PacketHandler m_PacketHandler = new PacketHandler ();
        protected ConnectionHandler m_ConnectionHandler = new ConnectionHandler ();
        protected ConnectListener m_Listener;
        protected BroadcastClient m_Broadcaster;

        private int m_usedConnectionId;

        public void ModuleInitialize () {
            m_ConnectionHandler.SetCallback (m_PacketHandler.GetNewPacket, null);
        }

        public void ModuleUninitialize () {
        }

        public void ModuleUpdate () {
            m_PacketHandler.ParseAllPacket ();
        }

        public void SetErrorCallback(ConnectionStatusCallback errorCallback) {
            m_ConnectionHandler.SetCallback(m_PacketHandler.GetNewPacket, errorCallback);
        }

        public void SetPacketHandlerDetail (PacketPackager packager, PacketParser[] parser) {
            m_PacketHandler.Packager = packager;
            for (int i = 0; i < parser.Length; i++) {
                m_PacketHandler.AddPacketParser(parser[i]);
            }
        }

        // *** Connector ***
        public ConnectClient GetConnector (int id) {
            return m_ConnectionHandler.GetConnector (id);
        }

        public void AddConnector (ConnectClient connectClient) {
            m_ConnectionHandler.AddConnector (connectClient);
        }

        public void AcceptConnection (int id, TcpClient client) {
            m_ConnectionHandler.AcceptConnection (id, client);
        }

        public void NewConnection (int id, string sIP, int iPort, ConnectionStatusCallback connectCallback) {
            m_ConnectionHandler.NewConnection(id, sIP, iPort, connectCallback);
        }

        public void RemoveConnection (int id) {
            m_ConnectionHandler.RemoveConnection (id);
        }

        public void Send (int id, byte[] aucPacket) {
            m_ConnectionHandler.Send (id, aucPacket);
        }

        // *** Listener ***
        public void SetListenDetail (int port, int conectionIdBegin) {
            m_usedConnectionId = conectionIdBegin;
            var localIP = IPAddressTool.GetLocalIP ();
            if (localIP != null) {
                m_Listener = new ConnectListener (localIP, port, AcceptConnection);
            } else {
                Util.UnityConsole.LogError ("[NetworkModule] LocalIP was not get.");
            }
        }
        public void StartListen () {
            if (m_Listener == null) {
                Util.UnityConsole.LogError ("[NetworkModule] Listener was not set.");
            } else {
                m_Listener.StartListen ();
            }
        }

        public void StopListen () {
            if (m_Listener == null) {
                Util.UnityConsole.LogError ("[NetworkModule] Listener was not set.");
            } else {
                m_Listener.StopListen ();
            }
        }

        private void AcceptConnection (TcpClient client) {
            m_ConnectionHandler.AcceptConnection (m_usedConnectionId, client);
            m_usedConnectionId += 1;
        }

        // *** Broadcast ***
        public void SetBroadcastPort(int port) {
            m_Broadcaster = new BroadcastClient (0, port, m_PacketHandler.GetNewPacket);
        }

        public void Broadcast (byte[] aucPacket) {
            m_Broadcaster.Broadcast (aucPacket);
        }
    }
}