using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib.EventManager.Old {
    public class EventObserver<T> where T : struct, IConvertible {

        private T m_eventCode;
        public T EventCode {
            get {
                return m_eventCode;
            }
        }

        private int m_ID;
        public int Id {
            get {
                return m_ID;
            }
        }

        private Action<T, EventArgs> m_callback;
        public Action<T, EventArgs> Callback {
            get {
                return m_callback;
            }
        }

        public EventObserver (T eventCode, Action<T, EventArgs> callback) {
            if (!typeof (T).IsEnum) {
                throw new ArgumentException ("T must be an enumerated type");
            }
            m_eventCode = eventCode;
            m_callback = callback;
        }

        public EventObserver<T> SetId (int id) {
            m_ID = id;
            return this;
        }
    }
}
