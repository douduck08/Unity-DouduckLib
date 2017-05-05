using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using DouduckGame.Util;

namespace DouduckGame.Network {
    public class PacketHandler {

        private struct ReceivePacket {
            public int clientId;
            public byte[] packet;
            public ReceivePacket(int id, byte[] aucPacket) {
                clientId = id;
                packet = aucPacket;
            }
        }

        private List<ReceivePacket> m_oReceivePackets = new List<ReceivePacket> ();
        private List<ReceivePacket> m_oParsingPackets;

        private PacketPackager m_PacketPackager;
        public PacketPackager Packager {
            set {
                m_PacketPackager = value;
            }
        }

        private List<PacketParser> m_Parsers = new List<PacketParser>();
        private Dictionary<uint, ReceiveCallback> m_parserDictionary = new Dictionary<uint, ReceiveCallback> ();

        public void AddPacketParser (PacketParser oPacketParser) {
            oPacketParser.Install (m_parserDictionary);
            m_Parsers.Add(oPacketParser);
        }

        public void RemovePacketParser (PacketParser oPacketParser) {
            oPacketParser.Uninstall ();
            m_Parsers.Remove (oPacketParser);
        }

        public void GetNewPacket (int id, byte[] aucPacket) {
            m_oReceivePackets.Add (new ReceivePacket(id, aucPacket));
        }

        public void ParseAllPacket () {
            lock (m_oReceivePackets) {
                m_oParsingPackets = new List<ReceivePacket> (m_oReceivePackets.ToArray ());
                m_oReceivePackets.Clear ();
            }
            for (int i = m_oParsingPackets.Count - 1; i >= 0; i--) {
                uint idx_ = m_PacketPackager.GetParserIndex (m_oParsingPackets[i].packet);
                if (m_parserDictionary.ContainsKey (idx_)) {
                    m_parserDictionary[idx_] (m_oParsingPackets[i].clientId, m_PacketPackager.GetData (m_oParsingPackets[i].packet));
                } else {
                    UnityConsole.LogError ("[PacketHandler] no parser can handle this index: " + idx_);
                }
            }
            m_oParsingPackets.Clear ();
        }
    }
}