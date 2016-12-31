using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DouduckGame {
	public delegate void PacketParseDelegate(Socket oSocket, byte[] aucData);
  public abstract class IPacketParser {
		private Dictionary<uint, PacketParseDelegate> m_oParsers = null;

		public void AddParser(uint uiProtocol, PacketParseDelegate dPacketHandle) {
			if (m_oParsers == null) {
				m_oParsers = new Dictionary<uint, PacketParseDelegate>();
			}
			if (m_oParsers.ContainsKey(uiProtocol)) {
				Debug.LogError("[IPacketParser] AddParser: Protocol conflict");
			} else {
				m_oParsers.Add(uiProtocol, dPacketHandle);
			}
		}

		public void Parse(uint index, Socket socket, byte[] packet) {
			if (m_oParsers.ContainsKey(index)) {
				m_oParsers[index](socket, packet);
			} else {
				Debug.LogError("[IPacketParser] Get valid index: " + index);
			}
		}
  }
}
