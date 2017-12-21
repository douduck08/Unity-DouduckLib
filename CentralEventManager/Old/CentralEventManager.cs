using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib.EventManager.Old {
    [Obsolete]
    public sealed class CentralEventManager {
        private static object m_oLock = new object ();
        private static CentralEventManager m_oInstance = null;
        private static CentralEventManager Instance {
            get {
                lock (m_oLock) {
                    if (m_oInstance == null) {
                        m_oInstance = new CentralEventManager ();
                    }
                    return m_oInstance;
                }
            }
        }

        private CentralEventManager () {

        }

        private Dictionary<Type, EventSubjectBase> m_SubjectDictionary = new Dictionary<Type, EventSubjectBase>();

        private EventSubject<T> GetSubject<T> () where T : struct, IConvertible {
            Type type_ = typeof (T);
            if (m_SubjectDictionary.ContainsKey (type_)) {
                return m_SubjectDictionary[type_] as EventSubject<T>;
            } else {
                EventSubject<T> subject_ = new EventSubject<T> ();
                m_SubjectDictionary.Add (type_, subject_);
                return subject_;
            }
        }

        public static EventObserver<T> RegisterCallback<T> (T eventCode, Action<T, EventArgs> observerCallback) where T : struct, IConvertible {
            return Instance.GetSubject<T> ().RegisterCallback (eventCode, observerCallback);
        }

        public static void UnregisterCallback<T> (T eventCode, Action<T, EventArgs> observerCallback) where T : struct, IConvertible {
            Instance.GetSubject<T> ().UnregisterCallback (eventCode, observerCallback);
        }

        public static void Notify<T> (T eventCode) where T : struct, IConvertible {
            Instance.GetSubject<T> ().Notify (eventCode, EventArgs.Empty);
        }

        public static void Notify<T> (T eventCode, EventArgs eventArgs) where T : struct, IConvertible {
            Instance.GetSubject<T> ().Notify (eventCode, eventArgs);
        }

        // more method to kill observers
        public static void KillObserverByEventId<T> (T eventCode) where T : struct, IConvertible {
            Instance.GetSubject<T> ().KillObserverByEventId (eventCode);
        }

        public static void KillObserverById<T> (int id) where T : struct, IConvertible {
            Instance.GetSubject<T> ().KillObserverById (id);
        }

        public static void KillObserverByTarget<T> (object target) where T : struct, IConvertible {
            Instance.GetSubject<T> ().KillObserverByTarget (target);
        }

        public static void KillAllObserver<T> () where T : struct, IConvertible {
            Instance.GetSubject<T> ().KillAllObserver ();
        }
    }
}
