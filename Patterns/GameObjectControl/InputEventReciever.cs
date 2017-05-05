using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckGame {
    [RequireComponent(typeof(GameObjectSet))]
    public class InputEventReciever : MonoBehaviour {

        public event Action<GameObject, Vector2> OnClick = null;
        public event Action<GameObject, string> OnButtonClick = null;
        public event Action<GameObject, Vector2> OnDrag = null;
        public event Action<GameObject, Vector2> OnEndDrag = null;

        public void Click (Vector2 pos) {
            if (OnClick != null) {
                OnClick (this.gameObject, pos);
            }
        }

        public void ButtonClick (string name) {
            if (OnButtonClick != null) {
                OnButtonClick (this.gameObject, name);
            }
        }

        public void Drag (Vector2 deltaPos) {
            if (OnEndDrag != null) {
                OnDrag (this.gameObject, deltaPos);
            }
        }

        public void EndDrag (Vector2 deltaPos) {
            if (OnEndDrag != null) {
                OnEndDrag (this.gameObject, deltaPos);
            }
        }
    }
}