using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DouduckGame.Util;

namespace DouduckGame.Network {
    public abstract class PacketParser {

        protected string m_name;
        public string Name {
            get {
                return m_name;
            }
        }

        private List<uint> m_installedList = new List<uint> ();
        private Dictionary<uint, ReceiveCallback> m_parserDictionary;

        public void Install (Dictionary<uint, ReceiveCallback> parserDictionary) {
            m_parserDictionary = parserDictionary;
            SetupParserCallback ();
        }

        public void Uninstall () {
            for (int i = 0; i < m_installedList.Count; i++) {
                m_parserDictionary.Remove (m_installedList[i]);
            }
            m_installedList.Clear ();
        }

        protected void AddParser (uint uiIndex, ReceiveCallback parseCallback) {
            if (m_parserDictionary.ContainsKey (uiIndex)) {
                UnityConsole.LogError ("[PacketParser] AddParser: Index conflict");
            } else {
                m_installedList.Add (uiIndex);
                m_parserDictionary.Add (uiIndex, parseCallback);
            }
        }

        protected abstract void SetupParserCallback ();
    }
}
