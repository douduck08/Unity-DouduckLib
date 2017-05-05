using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public sealed class SimpleTimer {

        private float m_fTarget;
        private float m_fTime;

        public float progress {
            get {
                if (m_fTarget > 0f) {
                    return m_fTime / m_fTarget;
                } else {
                    return 0f;
                }
            }
        }

        public SimpleTimer (float fTarget) {
            SetTarget (fTarget);
        }

        public void SetTarget (float fTarget) {
            if (fTarget < 0) {
                fTarget = 0f;
            }
            m_fTarget = fTarget;
            m_fTime = 0f;
        }

        public bool AddTime (float fDeltaTime) {
            m_fTime += fDeltaTime;
            if (m_fTime < m_fTarget) {
                return false;
            } else {
                m_fTime = 0f;
                return true;
            }
        }
    }
}
