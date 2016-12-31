using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public sealed class AdvancedGameStateController {

		private SceneController m_SceneController;
		private HierarchicalStateController m_StateController;

		public AdvancedGameStateController() {
			m_SceneController = new SceneController ();
			m_StateController = new HierarchicalStateController ();
		}

		public void SetScenePipeline(IScenePipeline oScenePipeline) {
			m_SceneController.SetScenePipeline (oScenePipeline);
		}

		public void Start(IHierarchicalState oState) {
			m_StateController.Start(oState);
		}

		public void Terminate() {
			m_StateController.Terminate();
		}

		public void TransTo(int iLevel, IHierarchicalState oState, string sSceneName = "", bool bReload = false) {
			m_StateController.TransTo(iLevel, oState);
			if (sSceneName != "") {
				m_SceneController.LoadScene(sSceneName, bReload);
			}
		}

		public void StateUpdate() {
			if (m_SceneController.isLoadingLevel) {
				m_SceneController.UpdateScenePipeline ();
			} else {
				m_StateController.StateUpdate();
			}
		}
	}
}