using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib.EventManager.Old {
    public abstract class EventSubjectBase {

    }

    public class EventSubject<T> : EventSubjectBase where T : struct, IConvertible {
        protected List<EventObserver<T>> m_Observers = new List<EventObserver<T>> ();
        private Dictionary<T, Action<T, EventArgs>> m_Callbacks = new Dictionary<T, Action<T, EventArgs>> ();

        public EventSubject () {
            if (!typeof (T).IsEnum) {
                throw new ArgumentException ("T must be an enumerated type");
            }
        }

        protected void AddCallback (T eventCode, Action<T, EventArgs> observerCallback) {
            if (m_Callbacks.ContainsKey (eventCode)) {
                m_Callbacks[eventCode] += observerCallback;
            } else {
                m_Callbacks.Add (eventCode, observerCallback);
            }
        }

        protected void RemoveCallback (T eventCode, Action<T, EventArgs> observerCallback) {
            if (m_Callbacks.ContainsKey (eventCode)) {
                m_Callbacks[eventCode] -= observerCallback;
                if (m_Callbacks[eventCode] == null) {
                    m_Callbacks.Remove (eventCode);
                }
            }
        }

        public virtual EventObserver<T> RegisterCallback (T eventCode, Action<T, EventArgs> observerCallback) {
            AddCallback (eventCode, observerCallback);
            EventObserver<T> newObserver_ = new EventObserver<T> (eventCode, observerCallback);
            m_Observers.Add (newObserver_);
            return newObserver_;
        }

        public virtual void UnregisterCallback (T eventCode, Action<T, EventArgs> observerCallback) {
            RemoveCallback (eventCode, observerCallback);
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].EventCode.Equals(eventCode) && m_Observers[i].Callback == observerCallback) {
                    m_Observers.RemoveAt (i);
                    return; // only remove one observer
                }
            }
        }

        public virtual void Notify (T eventCode, EventArgs eventArgs, bool useMultiple = false) {
            if (m_Callbacks.ContainsKey (eventCode)) {
                m_Callbacks[eventCode] (eventCode, eventArgs);
            }
        }

        public virtual void KillObserverByEventId (T eventCode) {
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].EventCode.Equals(eventCode)) {
                    m_Observers.RemoveAt (i);
                }
            }
            if (m_Callbacks.ContainsKey (eventCode)) {
                m_Callbacks.Remove (eventCode);
            }
        }

        public virtual void KillObserverById (int id) {
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].Id == id) {
                    RemoveCallback (m_Observers[i].EventCode, m_Observers[i].Callback);
                    m_Observers.RemoveAt (i);
                }
            }
        }

        public virtual void KillObserverByTarget (object target) {
            for (int i = m_Observers.Count - 1; i >= 0; i--) {
                if (m_Observers[i].Callback.Target == target) {
                    RemoveCallback (m_Observers[i].EventCode, m_Observers[i].Callback);
                    m_Observers.RemoveAt (i);
                }
            }
        }

        public virtual void KillAllObserver () {
            m_Observers.Clear ();
            m_Callbacks.Clear ();
        }
    }
}
