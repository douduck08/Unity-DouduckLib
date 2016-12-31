using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public class uGUI : MonoBehaviour {

		public string m_UIName;
		public string Name {
			get {
				if (m_UIName == null || m_UIName == "") {
					m_UIName = transform.name;
				}
				return m_UIName;
			}
			set {
				m_UIName = value;
			}
		}

		private IUserInterface m_UpperSystem = null;
		protected IUserInterface UpperSystem {
			get {
				if (m_UpperSystem == null) {
					m_UpperSystem = transform.GetComponentInParent<IUserInterface>();
					if (m_UpperSystem == null) {
						Debug.LogError("[uGUI] " + Name + ": UpperSystem was not found.");
					}
				}
				return m_UpperSystem;
			}
		}

		void Update () {
			OnUpdate();
		}

		void OnEnable () {
			OnActive();
		}

		void OnDisable () {
			OnInactive();
		}

		protected virtual void OnUpdate() {}
		protected virtual void OnActive() {}
		protected virtual void OnInactive() {}
	}
}
