using UnityEngine;
using System.Collections;

namespace DouduckGame.Network {
	public abstract class PacketPackager {
		public abstract uint GetParserIndex(byte[] aucPacket);
		public virtual byte[] GetData(byte[] aucPacket) {
			return aucPacket;
		}
	}
}
