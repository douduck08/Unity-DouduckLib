using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
	public sealed class GameSystemMonoManager {

		private bool m_bIsInitialized = false;

		private GameObject m_oContainer;
		private Dictionary<Type, IGameSystemMono> m_GameSystemList;

		// Initializaion method
		public GameSystemMonoManager (GameObject oContainer) {
			m_oContainer = oContainer;
			GameObject.DontDestroyOnLoad(m_oContainer);

			m_GameSystemList = new Dictionary<Type, IGameSystemMono> ();
		}

		public void StartInitialSystem() {
			if (m_bIsInitialized) {
				return;
			}
			m_bIsInitialized = true;

			IGameSystemMono[] systemList_ = m_oContainer.GetComponents<IGameSystemMono>();
			for (int i = 0; i < systemList_.Length; i++) {
				m_GameSystemList.Add(systemList_ [i].GetType(), systemList_ [i]);
				systemList_ [i].StartGameSystem();
			}
		}

		// Functional method
		public T AddSystem<T> () where T : IGameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				Debug.LogError("[GameSystemManager] There was a " + typeof(T).Name);
				return m_GameSystemList [typeof(T)] as T;
			} else {
                T gameSys_ = m_oContainer.AddComponent<T> ();
				gameSys_.StartGameSystem ();
				m_GameSystemList.Add(gameSys_.GetType(), gameSys_);
                return gameSys_;
			}
		}

		public void RemoveSystem<T> () where T : IGameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				IGameSystemMono gameSys_ = m_GameSystemList [typeof(T)];
				m_GameSystemList.Remove(typeof(T));
				gameSys_.DestoryGameSystem();
				GameObject.Destroy(gameSys_);
			} else {
				Debug.LogError("[GameSystemManager] There was no " + typeof(T).Name);
			}
		}

		public void EnableSystem<T> () where T : IGameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				m_GameSystemList [typeof(T)].enabled = true;
			} else {
				Debug.LogError ("[GameSystemManager] There was no " + typeof(T).Name);
			}
		}

		public void DisableSystem<T> () where T : IGameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				m_GameSystemList [typeof(T)].enabled = false;
			} else {
				Debug.LogError ("[GameSystemManager] There was no " + typeof(T).Name);
			}
		}

		public T GetSystem<T> () where T : IGameSystemMono {
			if (m_GameSystemList.ContainsKey(typeof(T))) {
				return m_GameSystemList [typeof(T)] as T;
			} else {
				Debug.LogError ("[GameSystemManager] There was no " + typeof(T).Name);
				return null;
			}
		}

	}
}
