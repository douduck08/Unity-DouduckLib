using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
	public sealed class GameSystemMonoManager {

		private bool m_bIsInitialized = false;

		private GameObject m_oContainer;
		private Dictionary<Type, GameSystemMono> m_GameSystemList;

		// Initializaion method
		public GameSystemMonoManager (GameObject oContainer) {
			m_oContainer = oContainer;
			GameObject.DontDestroyOnLoad(m_oContainer);

			m_GameSystemList = new Dictionary<Type, GameSystemMono> ();
		}

		public void StartInitialSystem() {
			if (m_bIsInitialized) {
				return;
			}
			m_bIsInitialized = true;

			GameSystemMono[] systemList_ = m_oContainer.GetComponents<GameSystemMono>();
			for (int i = 0; i < systemList_.Length; i++) {
				m_GameSystemList.Add(systemList_ [i].GetType(), systemList_ [i]);
				systemList_ [i].StartGameSystem();
			}
		}

		// Functional method
		public T AddSystem<T> () where T : GameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
                Util.UnityConsole.LogError("[GameSystemManager] There was a " + typeof(T).Name);
				return null;
			} else {
                T gameSys_ = m_oContainer.AddComponent<T> ();
				gameSys_.StartGameSystem ();
				m_GameSystemList.Add(gameSys_.GetType(), gameSys_);
                return gameSys_;
			}
		}

		public void RemoveSystem<T> () where T : GameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				GameSystemMono gameSys_ = m_GameSystemList [typeof(T)];
				m_GameSystemList.Remove(typeof(T));
				gameSys_.DestoryGameSystem();
				GameObject.Destroy(gameSys_);
			} else {
                Util.UnityConsole.LogError("[GameSystemManager] There was no " + typeof(T).Name);
			}
		}

		public void EnableSystem<T> () where T : GameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				m_GameSystemList [typeof(T)].enabled = true;
			} else {
                Util.UnityConsole.LogError ("[GameSystemManager] There was no " + typeof(T).Name);
			}
		}

		public void DisableSystem<T> () where T : GameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				m_GameSystemList [typeof(T)].enabled = false;
			} else {
                Util.UnityConsole.LogError ("[GameSystemManager] There was no " + typeof(T).Name);
			}
		}

		public T GetSystem<T> () where T : GameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				return m_GameSystemList [typeof(T)] as T;
			} else {
                Util.UnityConsole.LogError ("[GameSystemManager] There was no " + typeof(T).Name);
				return null;
			}
		}

	}
}
