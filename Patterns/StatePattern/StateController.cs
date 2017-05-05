using System.Collections;
using UnityEngine;

namespace DouduckGame {
    class StateController : StateControllerBase {
        private StateBase m_oCurrentState = null;
        public StateBase CurrentState {
            get {
                return m_oCurrentState;
            }
        }

        public StateController () { }
        public StateController (StateBase oStartState) : this() {
            TransTo (oStartState);
        }

        public void Terminate () {
            if (m_oCurrentState != null) {
                m_oCurrentState.StateEnd ();
                m_oCurrentState = null;
            }
        }

        public void TransTo (StateBase oState, int iLevel = 0) {
            if (m_oCurrentState != null) {
                m_oCurrentState.StateEnd ();
            }
            m_oCurrentState = oState;
            m_oCurrentState.SetController (this);
        }

        public void StateUpdate () {
            if (m_oCurrentState != null) {
                if (m_oCurrentState.AtStateBegin) {
                    m_oCurrentState.TouchBeginFlag ();
                    m_oCurrentState.StateBegin ();
                    if (m_oCurrentState == null || m_oCurrentState.AtStateBegin) {
                        return;
                    }
                }
                m_oCurrentState.StateUpdate ();
            }
        }
    }
}
