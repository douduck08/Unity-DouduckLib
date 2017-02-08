using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
    [DisallowMultipleComponent]
    [AddComponentMenu("DouduckGame/GameSystemMono/PlayerSystem")]
	public sealed class PlayerSystem : IGameSystemMono {
		
		public delegate void PlayerVisitor(IPlayer player);

		public const short MAX_PLAYER_NUM = 4;
		private List<IPlayer> m_PlayerList;
		public int PlayerCount {
			get {
				return m_PlayerList.Count;
			}
		}

		public override void StartGameSystem () {
			m_PlayerList = new List<IPlayer>();
		}

		public override void DestoryGameSystem () {
			foreach (IPlayer p_ in m_PlayerList) {
				p_.BeforeRemove ();
			}
			m_PlayerList.Clear ();
		}

		// void Update() {
		// TODO: Check if players lost connection
		// TODO: manage lost player, wait or remove
		// }

		public void VisitAllPlayer(PlayerVisitor visitor) {
			for (int i = m_PlayerList.Count - 1; i >= 0; i--) {
				visitor(m_PlayerList [i]);
			}
		}
			
		public IPlayer FindPlayerWithUID(uint uiUID) {
			return m_PlayerList.Find(p => p.UID == uiUID);
		}

		public IPlayer FindPlayerWithPlayerNum(int iPlayerNum) {
			return m_PlayerList.Find(p => p.PlayerNumber == iPlayerNum);
		}

		public int FindEmptyPlayerNum() {
			for (int i = 1; i <= MAX_PLAYER_NUM; i++) {
				if (FindPlayerWithPlayerNum(i) == null) {
					return i;
				}
			}
			return 0;
		}

		public void AddPlayer(IPlayer oPlayer) {
			if (oPlayer.PlayerNumber == 0 || oPlayer.UID == 0) {
				Debug.LogError("[PlayerSystem] PlayerNumber or UID is not setting");
				return;
			}
			if (FindPlayerWithUID(oPlayer.UID) != null) {
				Debug.LogError("[PlayerSystem] UID conflict: " + oPlayer.UID);
				return;
			}
			if (FindPlayerWithPlayerNum(oPlayer.PlayerNumber) != null) {
				Debug.LogError("[PlayerSystem] PlayerNumber conflict: " + oPlayer.PlayerNumber);
				return;
			}
			Debug.Log(string.Format("[PlayerSystem] Add Player, PlayerNumber = {0:}, UID = {1:}", oPlayer.PlayerNumber, oPlayer.UID));
			m_PlayerList.Add(oPlayer);
		}

		public void RemovePlayer(IPlayer oPlayer) {
			oPlayer.BeforeRemove ();
			m_PlayerList.Remove(oPlayer);
		}

		public void RemoveAllPlayer() {
			Debug.Log("[PlayerSystem] Remove All Player");
			foreach (IPlayer p_ in m_PlayerList) {
				p_.BeforeRemove ();
			}
			m_PlayerList.Clear ();
		}
	}
}