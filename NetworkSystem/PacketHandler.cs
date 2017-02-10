using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace DouduckGame.Network {
    public class PacketHandler {
        private struct ReceivePacket {
            public Socket socket;
            public byte[] packet;
            public ReceivePacket(Socket oSocket, byte[] aucPacket) {
                socket = oSocket;
                packet = aucPacket;
            }
        }

        private List<ReceivePacket> m_oReceivePackets = new List<ReceivePacket> ();
        private List<ReceivePacket> m_oParsingPackets;

        private PacketPackager m_PacketPackager;
        private List<PacketParser> m_Parsers = new List<PacketParser>();

        public void SetPacketPackager (PacketPackager oPacketPackager) {
            m_PacketPackager = oPacketPackager;
        }

        public void AddPacketParser (PacketParser oPacketParser) {
            m_Parsers.Add(oPacketParser);
        }

        public void GetNewPacket (Socket oSocket, byte[] aucPacket) {
            m_oReceivePackets.Add (new ReceivePacket(oSocket, aucPacket));
        }

        public void ParseAllPacket () {
            lock (m_oReceivePackets) {
                m_oParsingPackets = new List<ReceivePacket> (m_oReceivePackets.ToArray ());
                m_oReceivePackets.Clear ();
            }
            for (int i = m_oParsingPackets.Count - 1; i >= 0; i--) {
                uint idx_ = m_PacketPackager.GetParserIndex (m_oParsingPackets[i].packet);
                bool parseError_ = true;
                for (int j = 0; j < m_Parsers.Count; j++) {
                    if (m_Parsers[j].CheckIndex(idx_)) {
                        m_Parsers[j].Parse (idx_, m_oParsingPackets[i].socket, m_PacketPackager.GetData (m_oParsingPackets[i].packet));
                        parseError_ = false;
                        break;
                    }
                }
                if (parseError_) {
                    Debug.LogError ("[PacketHandler] no parser can handle this index: " + idx_);
                }
            }
            m_oParsingPackets.Clear ();
        }
    }
}