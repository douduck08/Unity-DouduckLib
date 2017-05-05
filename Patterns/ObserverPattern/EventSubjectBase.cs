using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public abstract class EventSubjectBase {

        protected bool m_bUseMultipleEventId = false;
        protected List<EventObserver> m_Observers = new List<EventObserver> ();
        private Dictionary<int, Action<int, EventArgs>> m_Callbacks = new Dictionary<int, Action<int, EventArgs>> ();

        public bool CheckObserver (int iEventId) {
            return m_Callbacks.ContainsKey (iEventId);
        }

        public virtual EventObserver RegisterCallback (int iEventId, Action<int, EventArgs> observerCallback) {
            AddCallback (iEventId, observerCallback);
            EventObserver newObserver_ = new EventObserver (iEventId, observerCallback);
            m_Observers.Add (newObserver_);
            return newObserver_;
        }

        public virtual void UnregisterCallback (int iEventId, Action<int, EventArgs> observerCallback) {
            RemoveCallback (iEventId, observerCallback);
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].EventId == iEventId && m_Observers[i].Callback == observerCallback) {
                    m_Observers.RemoveAt (i);
                    break; // only remove one observer
                }
            }
        }

        public virtual void Notify (int iEventId, EventArgs eventArgs) {
            if (m_bUseMultipleEventId) {
                foreach (int Key in m_Callbacks.Keys) {
                    if ((Key & iEventId) == iEventId) {
                        m_Callbacks[Key] (iEventId, eventArgs);
                    }
                }
            } else {
                if (m_Callbacks.ContainsKey (iEventId)) {
                    m_Callbacks[iEventId] (iEventId, eventArgs);
                }
            }
        }

        public virtual void KillObserverByEventId (int iEventId) {
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].EventId == iEventId) {
                    m_Observers.RemoveAt (i);
                }
            }
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
            m_Observers.Clear ();
            m_Callbacks.Clear ();
        }

        protected void AddCallback (int iEventId, Action<int, EventArgs> observerCallback) {
            if (m_Callbacks.ContainsKey (iEventId)) {
                m_Callbacks[iEventId] += observerCallback;
            } else {
                m_Callbacks.Add (iEventId, observerCallback);
            }
        }

        protected void RemoveCallback (int iEventId, Action<int, EventArgs> observerCallback) {
            if (m_Callbacks.ContainsKey (iEventId)) {
                m_Callbacks[iEventId] -= observerCallback;
                if (m_Callbacks[iEventId] == null) {
                    m_Callbacks.Remove (iEventId);
                }
            }
        }
    }
}
