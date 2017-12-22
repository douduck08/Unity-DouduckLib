using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib.EventManager {
    public abstract class SubjectBase {}
    public abstract class SubjectBase<TSubject> { }

    public abstract class Subject<TValue> : SubjectBase {
        private Action<TValue> m_observers;

        public IDisposable Subscribe (Action<TValue> observer) {
            return new Subscription (m_observers, observer);
        }

        public void Notify (TValue value) {
            if (m_observers != null) m_observers.Invoke (value);
        }

        class Subscription : IDisposable {
            private Action<TValue> m_source;
            private Action<TValue> m_target;

            public Subscription (Action<TValue> source, Action<TValue> target) {
                source += target;
                m_source = source;
                m_target = target;
            }

            public void Dispose () {
                m_source -= m_target;
            }
        }
    }

    public abstract class SubjectGroup<TSubjectGroup> : SubjectBase where TSubjectGroup : SubjectGroup<TSubjectGroup> {
        private Dictionary<Type, SubjectBase<TSubjectGroup>> m_subEventDictionary = new Dictionary<Type, SubjectBase<TSubjectGroup>> ();

        public T GetSubject<T>() where T : SubjectBase<TSubjectGroup>, new() {
            Type type_ = typeof (T);
            if (m_subEventDictionary.ContainsKey (type_)) {
                return (T)(m_subEventDictionary[type_]);
            } else {
                T subject_ = new T ();
                m_subEventDictionary.Add (type_, subject_);
                return subject_;
            }
        }

        public IDisposable Subscribe<TSubject, TValue>(Action<TValue> observer) where TSubject : Subject<TSubjectGroup, TValue>, new() {
            return GetSubject<TSubject> ().Subscribe (observer);
        }

        public void Notify<TSubject, TValue>(TValue value) where TSubject : Subject<TSubjectGroup, TValue>, new() {
            this.GetSubject<TSubject> ().Notify (value);
        }

        public IDisposable Subscribe<TSubject>(Action<TSubject> observer) where TSubject : Subject<TSubjectGroup, TSubject>, new() {
            return GetSubject<TSubject> ().Subscribe (observer);
        }

        public void Notify<TSubject>(TSubject value) where TSubject : Subject<TSubjectGroup, TSubject>, new() {
            this.GetSubject<TSubject> ().Notify (value);
        }
    }

    public abstract class Subject<TSubjectGroup, TValue> : SubjectBase<TSubjectGroup> where TSubjectGroup : SubjectGroup<TSubjectGroup> {
        Action<TValue> m_observers;

        public IDisposable Subscribe (Action<TValue> observer) {
            return new Subscription (m_observers, observer);
        }

        public void Notify (TValue value) {
            if (m_observers != null) m_observers.Invoke (value);
        }

        class Subscription : IDisposable {
            Action<TValue> m_source;
            Action<TValue> m_target;

            public Subscription (Action<TValue> source, Action<TValue> target) {
                source += target;
                m_source = source;
                m_target = target;
            }

            public void Dispose () {
                m_source -= m_target;
            }
        }
    }
}
