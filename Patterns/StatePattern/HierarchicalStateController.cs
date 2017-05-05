using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    class HierarchicalStateController : StateControllerBase {
        private List<StateBase> m_oCurrentState = null;
        public List<StateBase> CurrentState {
            get {
                return m_oCurrentState;
            }
        }

        public HierarchicalStateController () {
            m_oCurrentState = new List<StateBase> ();
        }
        public HierarchicalStateController (StateBase oStartState) : this() {
            TransTo (oStartState);
        }

        public void Terminate () {
            for (int i = m_oCurrentState.Count - 1; i >= 0; i--) {
                m_oCurrentState[i].StateEnd ();
            }
            m_oCurrentState.Clear ();
        }

        public void TransTo (StateBase oState, int iLevel = 0) {
            if (iLevel > m_oCurrentState.Count) {
                Util.UnityConsole.LogError ("[StateController] Level is too big");
                return;
            }

            Util.UnityConsole.Log (string.Format ("[StateController] Level {0:} transTo: {1:}", iLevel, oState.ToString ()));
            if (iLevel == m_oCurrentState.Count) {
                m_oCurrentState.Add (oState);
                m_oCurrentState[iLevel].SetController (this);
                m_oCurrentState[iLevel].SetLevel (iLevel);
            } else {
                for (int i = m_oCurrentState.Count - 1; i >= iLevel; i--) {
                    m_oCurrentState[i].StateEnd ();
                    m_oCurrentState.RemoveAt (i);
                }
                m_oCurrentState.Add (oState);
                m_oCurrentState[iLevel].SetController (this);
                m_oCurrentState[iLevel].SetLevel (iLevel);
            }
        }

        public void StateUpdate () {
            for (int i = 0; i < m_oCurrentState.Count; i++) {
                if (m_oCurrentState[i].AtStateBegin) {
                    m_oCurrentState[i].TouchBeginFlag ();
                    m_oCurrentState[i].StateBegin ();
                    if (m_oCurrentState[i] == null || m_oCurrentState[i].AtStateBegin) {
                        return;
                    }
                }
                m_oCurrentState[i].StateUpdate ();
            }
        }
    }
}
