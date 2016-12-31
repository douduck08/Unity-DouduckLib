using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class IState {
		private StateController m_StateController;
		protected StateController Controller {
			get {
				return m_StateController;
			}
		}
		private bool m_bAtStateBegin = true;
		public bool AtStateBegin {
			get {
				return m_bAtStateBegin;
			}
		}
			
		public void SetProperty(StateController oController) {
			m_StateController = oController;
		}

		public void TouchStateBegin() {
			m_bAtStateBegin = false;
		}

		protected void TransTo(IState oState) {
			m_StateController.TransTo(oState);
		}

		public virtual void StateBegin() {}
		public virtual void StateUpdate() {}
		public virtual void StateEnd() {}

		public override string ToString () {
			return string.Format ("<IState>" + this.GetType().Name);
		}
	}
}
