using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public class SimpleSubject {

        protected List<SimpleObserver> m_Observers = new List<SimpleObserver> ();
        private Dictionary<int, Action<int>> m_Callbacks = new Dictionary<int, Action<int>> ();

        public bool CheckObserver (int iEventId) {
            return m_Callbacks.ContainsKey (iEventId);
        }

        public virtual SimpleObserver RegisterObserver (int iEventId, Action<int> observerCallback) {
            AddCallback (iEventId, observerCallback);
            SimpleObserver newObserver_ = new SimpleObserver (iEventId, observerCallback);
            m_Observers.Add (newObserver_);
            return newObserver_;
        }

        public virtual void UnregisterObserver (int iEventId, Action<int> observerCallback) {
            RemoveCallback (iEventId, observerCallback);
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].EventId == iEventId && m_Observers[i].Callback == observerCallback) {
                    m_Observers.RemoveAt (i);
                    break; // only remove one observer
                }
            }
        }

        public virtual void Notify (int iEventId) {
            if (m_Callbacks.ContainsKey (iEventId)) {
                m_Callbacks[iEventId] (iEventId);
            }
        }

        public virtual void KillObserverByEventId (int iEventId) {
            if (m_Callbacks.ContainsKey (iEventId)) {
                m_Callbacks.Remove (iEventId);
            }
        }

        public virtual void KillObserverById (int iId) {
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].Id == iId) {
                    RemoveCallback (m_Observers[i].EventId, m_Observers[i].Callback);
                    m_Observers.RemoveAt (i);
                }
            }
        }

        public virtual void KillObserverByTarget (object target) {
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].Callback.Target == target) {
                    RemoveCallback (m_Observers[i].EventId, m_Observers[i].Callback);
                    m_Observers.RemoveAt (i);
                }
            }
        }

        public virtual void KillAllObserver () {
            m_Callbacks.Clear ();
        }

        protected void AddCallback (int iEventId, Action<int> observerCallback) {
            if (m_Callbacks.ContainsKey (iEventId)) {
                m_Callbacks[iEventId] += observerCallback;
            } else {
                m_Callbacks.Add (iEventId, observerCallback);
            }
        }

        protected void RemoveCallback (int iEventId, Action<int> observerCallback) {
            if (m_Callbacks.ContainsKey (iEventId)) {
                m_Callbacks[iEventId] -= observerCallback;
                if (m_Callbacks[iEventId] == null) {
                    m_Callbacks.Remove (iEventId);
                }
            }
        }
    }
}