using UnityEngine;
using System;
using System.Collections;

namespace DouduckGame {
	public abstract class IGUIUnit : MonoBehaviour {

		public string m_UIName;
		public string Name {
			get {
				if (m_UIName == null || m_UIName == "") {
					m_UIName = transform.name;
				}
				return m_UIName;
			}
		}
		private bool m_bActive = true;
		public bool IsVisible {
			get {
				return m_bActive;
			}
		}

        public event Action<GameObject, Vector2> OnClick = null;
        public event Action<GameObject, string> OnButtonClick = null;
        public event Action<GameObject, float, float> OnEndDrag = null;

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

        public void EndDrag (float deltaX, float deltaY) {
            if (OnEndDrag != null) {
                OnEndDrag (this.gameObject, deltaX, deltaY);
            }
        }

        public void Active() {
			this.gameObject.SetActive(true);
			m_bActive = true;
			OnShow();
		}

		public void Inactive() {
			this.gameObject.SetActive(false);
			m_bActive = false;
			OnHide();
		}

		public void Show() {
			// TODO
		}

		public void Hide() {
			// TODO
		}

		public virtual void OnShow() {}
		public virtual void OnHide() {}
	}
}
