using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public class EventObserver {

        private int m_iEventID;
        public int EventId {
            get {
                return m_iEventID;
            }
        }

        private int m_iID;
        public int Id {
            get {
                return m_iID;
            }
        }

        private Action<int, EventArgs> m_dCallback;
        public Action<int, EventArgs> Callback {
            get {
                return m_dCallback;
            }
        }

        public EventObserver (int iEventID, Action<int, EventArgs> callback) {
            m_iEventID = iEventID;
            m_dCallback = callback;
        }

        public EventObserver SetId (int id) {
            m_iID = id;
            return this;
        }

    }
}
