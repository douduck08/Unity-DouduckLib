using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DouduckGame {
	public sealed class GameEventSystem : IGameSystemMono {

		private Dictionary<Type, IGameEventGroup> m_SubjectDictionary = null;

		public override void StartGameSystem () {
			m_SubjectDictionary = new Dictionary<Type, IGameEventGroup> ();
		}

		public override void DestoryGameSystem () {
			foreach (KeyValuePair<Type, IGameEventGroup> p_ in m_SubjectDictionary) {
				p_.Value.ClearAllObserver ();
			}
			m_SubjectDictionary.Clear ();
		}

		private T GetSubject<T> () where T : IGameEventGroup, new() {
			Type type_ = typeof(T);
			if (m_SubjectDictionary.ContainsKey (type_)) {
				return m_SubjectDictionary [type_] as T;
			} else {
				T subject_ = new T ();
				m_SubjectDictionary.Add (type_, subject_);
				return subject_;
			}
		}

		public void RegisterObserver<T> (int iEventId, Action<int> dObserverCallback) where T : IGameEventGroup, new() {
			T subject_ = GetSubject<T>();
			subject_.RegisterObserver (iEventId, dObserverCallback);
		}

		public void UnregisterObserver<T> (int iEventId, Action<int> dObserverCallback) where T : IGameEventGroup, new() {
			T subject_ = GetSubject<T>();
			subject_.UnregisterObserver (iEventId, dObserverCallback);
		}

		public void Notify<T> (int iEventId) where T : IGameEventGroup, new() {
			T subject_ = GetSubject<T>();
			subject_.Notify (iEventId);
		}

		public void ClearObserver<T> (int iEventId) where T : IGameEventGroup, new() {
			T subject_ = GetSubject<T>();
			subject_.ClearObserver (iEventId);
		}
	}
}