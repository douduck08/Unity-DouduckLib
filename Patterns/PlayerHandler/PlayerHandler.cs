using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
    public sealed class PlayerHandler<T> where T : Player {
        private short m_maxPlayerNumber;
        public short MaxPlayerNumber {
            get {
                return m_maxPlayerNumber;
            }
        }

        private List<T> m_PlayerList;
        public int PlayerCount {
            get {
                return m_PlayerList.Count;
            }
        }

        public PlayerHandler (short maxPlayerNumber) {
            m_maxPlayerNumber = maxPlayerNumber;
            m_PlayerList = new List<T> ();
        }

        public void VisitAllPlayer (System.Action<T> visitor) {
            for (int i = m_PlayerList.Count - 1; i >= 0; i--) {
                visitor (m_PlayerList[i]);
            }
        }

        public int FindEmptyPosition () {
            for (int i = 1; i <= m_maxPlayerNumber; i++) {
                if (FindPlayerWithPlayerNum (i) == null) {
                    return i;
                }
            }
            return 0;
        }

        public T FindPlayerWithPlayerNum (int iPlayerNum) {
            return m_PlayerList.Find (p => p.PlayerNumber == iPlayerNum);
        }

        public void AddPlayer (T oPlayer) {
            if (FindPlayerWithPlayerNum (oPlayer.PlayerNumber) != null) {
                Util.UnityConsole.LogError ("[PlayerSystem] PlayerNumber conflict: " + oPlayer.PlayerNumber);
                return;
            }
            Util.UnityConsole.Log ("[PlayerSystem] Add Player " + oPlayer.ToString ());
            m_PlayerList.Add (oPlayer);
        }

        public void RemovePlayer (T oPlayer) {
            oPlayer.BeforeRemove ();
            m_PlayerList.Remove (oPlayer);
        }

        public void RemoveAllPlayer () {
            Util.UnityConsole.Log ("[PlayerSystem] Remove All Player");
            foreach (T p_ in m_PlayerList) {
                p_.BeforeRemove ();
            }
            m_PlayerList.Clear ();
        }
    }
}