using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib.EventManager {
    public abstract class SubjectBase {}
    public abstract class Subject<TSource, TResult> : SubjectBase {

        private Action<TResult> m_observers;

        public IDisposable Subscribe (Action<TResult> observer) {
            m_observers += observer;
            return new Subscription(m_observers, observer);
        }

        public void Notify (TResult value) {
            if (m_observers != null) {
                m_observers.Invoke (value);
            }
        }

        class Subscription : IDisposable {
            Action<TResult> m_source;
            Action<TResult> m_target;
            public Subscription (Action<TResult> source, Action<TResult> target) {
                m_source = source;
                m_target = target;
            }

            public void Dispose () {
                m_source -= m_target;
            }
        }
    }
}
