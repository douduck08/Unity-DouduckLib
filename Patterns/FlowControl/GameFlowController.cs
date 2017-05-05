using UnityEngine;
using System.Collections;

namespace DouduckGame.GameFlow {
	public sealed class GameFlowController {

		private SceneController m_SceneController;
		private HierarchicalStateController m_StateController;

		public GameFlowController () {
			m_SceneController = new SceneController ();
			m_StateController = new HierarchicalStateController ();
		}

		public void Terminate() {
			m_StateController.Terminate();
		}
        
        public GameFlowController TransTo (State oState, int iLevel = 0) {
			m_StateController.TransTo(oState, iLevel);
            return this;
        }

        public GameFlowController LoadScene (string sSceneName) {
            m_SceneController.LoadScene (sSceneName);
            return this;
        }

        public void StateUpdate() {
			if (!m_SceneController.isLoadingLevel) {
				m_StateController.StateUpdate();
			}
		}
	}
}