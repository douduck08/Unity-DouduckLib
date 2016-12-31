using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public sealed class GameStateController {

		private SceneController m_SceneController;
		private StateController m_StateController;

		public GameStateController() {
			m_SceneController = new SceneController ();
			m_StateController = new StateController ();
		}

		public void SetScenePipeline(IScenePipeline oScenePipeline) {
			m_SceneController.SetScenePipeline (oScenePipeline);
		}

		public void Start(IState oState) {
			m_StateController.Start(oState);
		}

		public void Terminate() {
			m_StateController.Terminate();
		}

		public void TransTo(IState oState, string sSceneName = "", bool bReload = false) {
			m_StateController.TransTo(oState);
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