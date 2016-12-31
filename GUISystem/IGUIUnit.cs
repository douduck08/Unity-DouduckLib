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

		public event Action<GameObject, Vector2> OnClick;
		public event Action<GameObject, string> OnButtonClick;
		public event Action<GameObject, int, float> OnDrag;
		public event Action<GameObject, int, float> OnEndDrag;

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
