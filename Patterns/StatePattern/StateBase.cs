using System.Collections;
using UnityEngine;

namespace DouduckGame {
    public class StateBase {
        private StateControllerBase m_StateController;
        protected StateControllerBase Controller {
            get {
                return m_StateController;
            }
        }

        private int m_iLevel = 0;
        protected int StateLevel {
            get {
                return m_iLevel;
            }
        }

        private bool m_bAtStateBegin = true;
        public bool AtStateBegin {
            get {
                return m_bAtStateBegin;
            }
        }

        public void SetController (StateControllerBase controller) {
            m_StateController = controller;
        }

        public void SetLevel (int iLevel) {
            m_iLevel = iLevel;
        }

        public void TouchBeginFlag () {
            m_bAtStateBegin = false;
        }

        public virtual void StateBegin () { }
        public virtual void StateUpdate () { }
        public virtual void StateEnd () { }

        public override string ToString () {
            return string.Format ("<State>" + this.GetType ().Name);
        }
    }
}
