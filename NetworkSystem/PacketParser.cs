using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DouduckGame.Network {
    public abstract class PacketParser {
        private Dictionary<uint, PacketParseCallback> m_oParsers = new Dictionary<uint, PacketParseCallback> ();
        protected void AddParser (uint uiIndex, PacketParseCallback parseCallback) {
            if (m_oParsers.ContainsKey (uiIndex)) {
                Debug.LogError ("[PacketParser] AddParser: Index conflict");
            } else {
                m_oParsers.Add (uiIndex, parseCallback);
            }
        }

        public bool CheckIndex (uint uiIndex) {
            return m_oParsers.ContainsKey (uiIndex);
        }

        public void Parse (uint uiIndex, Socket socket, byte[] packet) {
            if (m_oParsers.ContainsKey (uiIndex)) {
                m_oParsers[uiIndex] (socket, packet);
            } else {
                Debug.LogError ("[PacketParser] Get valid index: " + uiIndex);
            }
        }
    }
}
