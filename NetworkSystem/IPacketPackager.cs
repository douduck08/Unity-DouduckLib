using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class IPacketPackager {
		public abstract uint GetParserIndex(byte[] aucPacket);
		public virtual byte[] GetData(byte[] aucPacket) {
			return aucPacket;
		}
	}
}
