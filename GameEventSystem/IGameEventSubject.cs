using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
	public abstract class IGameEventGroup {
		private Dictionary<int, Action<int>> m_Observers = new Dictionary<int, Action<int>>();

		public void RegisterObserver (int iEventId, Action<int> dObserverCallback) {
			if (m_Observers.ContainsKey (iEventId)) {
				m_Observers [iEventId] += dObserverCallback;
			} else {
				m_Observers.Add (iEventId, dObserverCallback);
			}
		}

		public void UnregisterObserver(int iEventId, Action<int> dObserverCallback) {
			if (m_Observers.ContainsKey (iEventId)) {
				m_Observers [iEventId] -= dObserverCallback;
				if (m_Observers [iEventId] == null) {
					m_Observers.Remove (iEventId);
				}
			}
		}

		public void Notify(int iEventId) {
			Debug.Log (string.Format ("[IGameEventGroup] Notify {0:}, Event Id = {1:}", this.GetType().Name, iEventId));
			if (m_Observers.ContainsKey (iEventId)) {
				m_Observers [iEventId] (iEventId);
			}
		}

		public void ClearObserver(int iEventId) {
			if (m_Observers.ContainsKey (iEventId)) {
				m_Observers.Remove (iEventId);
			}
		}

		public void ClearAllObserver() {
			m_Observers.Clear ();
		}
	}
}