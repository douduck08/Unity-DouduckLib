using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    [DisallowMultipleComponent]
    [AddComponentMenu("DouduckGame/GameSystemMono/CentralEventSystem")]
    public sealed class CentralEventSystem : IGameSystemMono {

        private Dictionary<Type, EventSubjectBase> m_SubjectDictionary = null;

        public override void StartGameSystem () {
            m_SubjectDictionary = new Dictionary<Type, EventSubjectBase> ();
        }

        public override void DestoryGameSystem () {
            foreach (KeyValuePair<Type, EventSubjectBase> p_ in m_SubjectDictionary) {
                p_.Value.KillAllObserver ();
            }
            m_SubjectDictionary.Clear ();
        }

        private T GetSubject<T>() where T : EventSubjectBase, new() {
            Type type_ = typeof (T);
            if (m_SubjectDictionary.ContainsKey (type_)) {
                return m_SubjectDictionary[type_] as T;
            } else {
                T subject_ = new T ();
                m_SubjectDictionary.Add (type_, subject_);
                return subject_;
            }
        }

        public void RegisterCallback<T>(int iEventId, Action<int, EventArgs> observerCallback) where T : EventSubjectBase, new() {
            GetSubject<T> ().RegisterCallback (iEventId, observerCallback);
        }

        public void UnregisterCallback<T>(int iEventId, Action<int, EventArgs> observerCallback) where T : EventSubjectBase, new() {
            GetSubject<T> ().UnregisterCallback (iEventId, observerCallback);
        }

        public void Notify<T>(int iEventId, EventArgs eventArgs) where T : EventSubjectBase, new() {
            GetSubject<T> ().Notify (iEventId, eventArgs);
        }

        public void KillObserverByEventId<T>(int iEventId) where T : EventSubjectBase, new() {
            GetSubject<T> ().KillObserverByEventId (iEventId);
        }

        public void KillObserverById<T>(int iId) where T : EventSubjectBase, new() {
            GetSubject<T> ().KillObserverById (iId);
        }

        public void KillObserverByTarget<T>(object target) where T : EventSubjectBase, new() {
            GetSubject<T> ().KillObserverByTarget (target);
        }

        public void KillAllObserver<T>() where T : EventSubjectBase, new() {
            GetSubject<T> ().KillAllObserver ();
        }
    }
}
