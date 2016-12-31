using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class IUserInterface : MonoBehaviour {
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
			set {
				if (value) {
					Show();
				} else {
					Hide();
				}
			}
		}

		public void Show() {
			this.gameObject.SetActive(true);
			OnShow();
			m_bActive = true;
		}

		public void Hide() {
			this.gameObject.SetActive(false);
			OnHide();
			m_bActive = false;
		}

		public virtual void OnShow() {}
		public virtual void OnHide() {}

		public virtual void OnButtonClick (string sButtonName, GameObject oGameObject) {}
		public virtual void OnDrag (int iDirection, float fLength, GameObject oGameObject) {}
		public virtual void OnEndDrag (int iDirection, float fLength, GameObject oGameObject) {}

	}
}