using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class IGUIComponent : MonoBehaviour {

		public string Name {
			get {
				return transform.name;
			}
		}

		private IGUIUnit m_UpperUnit = null;
		protected IGUIUnit UpperUnit {
			get {
				if (m_UpperUnit == null) {
					m_UpperUnit = transform.GetComponentInParent<IGUIUnit>();
					if (m_UpperUnit == null) {
						Debug.LogError(string.Format("[IGUIComponent] {0:}: UpperUnit was not found.", Name));
					}
				}
				return m_UpperUnit;
			}
		}
	}
}
