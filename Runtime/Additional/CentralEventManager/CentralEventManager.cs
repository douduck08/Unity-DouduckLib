using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib.EventManager {
    public class CentralEventManager {

        private static object m_oLock = new object ();
        private static CentralEventManager m_instance = null;
        private static CentralEventManager instance {
            get {
                lock (m_oLock) {
                    if (m_instance == null) {
                        m_instance = new CentralEventManager ();
                    }
                    return m_instance;
                }
            }
        }

        private Dictionary<Type, SubjectBase> m_subjectDictionary = new Dictionary<Type, SubjectBase> ();
        private Dictionary<Type, SimpleSubjectBase> m_simpleSubjectDictionary = new Dictionary<Type, SimpleSubjectBase> ();

        public static T GetSubject<T> () where T : SubjectBase, new() {
            Type type_ = typeof (T);
            if (instance.m_subjectDictionary.ContainsKey (type_)) {
                return (T)(instance.m_subjectDictionary[type_]);
            } else {
                T subject_ = new T ();
                instance.m_subjectDictionary.Add (type_, subject_);
                return subject_;
            }
        }

        private SimpleSubject<T> GetSimpleSubject<T> () where T : struct, IConvertible {
            Type type_ = typeof (T);
            if (m_simpleSubjectDictionary.ContainsKey (type_)) {
                return (SimpleSubject<T>)(m_simpleSubjectDictionary[type_]);
            } else {
                SimpleSubject<T> subject_ = new SimpleSubject<T> ();
                m_simpleSubjectDictionary.Add (type_, subject_);
                return subject_;
            }
        }

        public static IDisposable SimpleSubscribe<T> (T eventCode, Action<T> observer) where T : struct, IConvertible {
            return instance.GetSimpleSubject<T> ().Subscribe (eventCode, observer);
        }

        public static void SimpleNotify<T>(T eventCode) where T : struct, IConvertible {
            instance.GetSimpleSubject<T> ().Notify (eventCode);
        }
    }
}