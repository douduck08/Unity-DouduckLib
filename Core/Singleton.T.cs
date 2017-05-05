using UnityEngine;
using System.Collections;

namespace DouduckGame {
    public class Singleton<T> where T : class, new() {
        private static object m_oLock = new object ();
        private static T m_oInstance = null;
        public static T Instance {
            get {
                lock (m_oLock) {
                    if (m_oInstance == null) {
                        m_oInstance = new T ();
                    }
                    return m_oInstance;
                }
            }
        }
    }
}
