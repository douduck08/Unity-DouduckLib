using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public abstract class InputEventSender : MonoBehaviour {
        private InputEventReciever m_inputEventReciever = null;
        protected InputEventReciever EventReciever {
            get {
                if (m_inputEventReciever == null) {
                    m_inputEventReciever = transform.GetComponentInParent<InputEventReciever> ();
                    if (m_inputEventReciever == null) {
                        Util.UnityConsole.LogError (string.Format ("[InputEventSender] {0:}: InputEventReciever was not found.", transform.name));
                    }
                }
                return m_inputEventReciever;
            }
        }
    }
}