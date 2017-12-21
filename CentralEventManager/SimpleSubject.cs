using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib.EventManager {
    public abstract class SimpleSubjectBase { }
    public sealed class SimpleSubject<T> : SimpleSubjectBase where T : struct, IConvertible {
        private Dictionary<T, Action<T>> m_observers = new Dictionary<T, Action<T>> ();

        public SimpleSubject () {
            if (!typeof (T).IsEnum) {
                throw new ArgumentException ("T must be an enumerated type");
            }
        }

        public IDisposable Subscribe (T eventCode, Action<T> observer) {
            if (m_observers.ContainsKey (eventCode)) {
                m_observers[eventCode] += observer;
            } else {
                m_observers.Add (eventCode, observer);
            }
            return new Subscription (m_observers[eventCode], observer);
        }

        public void Notify (T eventCode) {
            if (m_observers.ContainsKey (eventCode) && m_observers[eventCode] != null) {
                m_observers[eventCode] (eventCode);
            }
        }

        class Subscription : IDisposable {
            Action<T> m_source;
            Action<T> m_target;
            public Subscription (Action<T> source, Action<T> target) {
                m_source = source;
                m_target = target;
            }

            public void Dispose () {
                m_source -= m_target;
            }
        }
    }
}