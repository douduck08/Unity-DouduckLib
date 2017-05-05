using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public abstract class DataState<T> : StateBase {
        protected T m_Data;
        public void Injection (T data) {
            m_Data = data;
        }

        protected void TransTo (DataState<T> state) {
            state.Injection (m_Data);
            Controller.TransTo (state, StateLevel);
        }
    }
}