using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DouduckGame {
	public class PacketManager : Singleton<PacketManager> {
		private struct ReceivePacket {
			public Socket socket;
			public byte[] packet;
		}
		private List<ReceivePacket> m_oReceivePackets = null;
		private List<ReceivePacket> m_oParsingPackets;
		private IPacketParser m_PacketParser;
		private IPacketPackager m_PacketPackager;
		public PacketManager() {
			m_oReceivePackets = new List<ReceivePacket>();
		}

		public void SetPacketPackager(IPacketPackager oPacketPackager) {
			m_PacketPackager = oPacketPackager;
		}

		public void SetPacketParser(IPacketParser oPacketParser) {
			m_PacketParser = oPacketParser;
		}

		public void NewPacket(Socket oSocket, byte[] aucPacket) {
			ReceivePacket tReceivePacket_;
			tReceivePacket_.socket = oSocket;
			tReceivePacket_.packet = aucPacket;
			m_oReceivePackets.Add(tReceivePacket_);
		}

		public void ParseAllPacket() {
			lock (m_oReceivePackets) {
				m_oParsingPackets = new List<ReceivePacket>(m_oReceivePackets.ToArray());
				m_oReceivePackets.Clear();
			}
			uint idx_;
			for (int i = m_oParsingPackets.Count - 1; i >= 0; i--) {
				idx_ = m_PacketPackager.GetParserIndex(m_oParsingPackets[i].packet);
				m_PacketParser.Parse(idx_, m_oParsingPackets[i].socket, m_PacketPackager.GetData(m_oParsingPackets[i].packet));
			}
			m_oParsingPackets.Clear();
		}
	}
}
