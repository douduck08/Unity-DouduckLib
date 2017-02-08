using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    [DisallowMultipleComponent]
    [AddComponentMenu("DouduckGame/GameSystemMono/GameFlowSystem")]
	public class GameFlowSystem : IGameSystemMono {

		public System.Type Test;

		private AdvancedGameStateController m_GameStateController;

		public override void StartGameSystem () {
			m_GameStateController = new AdvancedGameStateController ();
		}

		public override void DestoryGameSystem () {

		}

		void Update() {
			m_GameStateController.StateUpdate ();
		}

		public void SetScenePipeline(IScenePipeline oScenePipeline) {
			m_GameStateController.SetScenePipeline(oScenePipeline);
		}

		public void StartFirstState(IHierarchicalState oState) {
			m_GameStateController.Start(oState);
		}

		public void Terminate() {
			m_GameStateController.Terminate();
			m_GameStateController = new AdvancedGameStateController ();
		}

		public void TransTo(int iLevel, IHierarchicalState oState, string sSceneName = "", bool bReload = false) {
			m_GameStateController.TransTo(iLevel, oState, sSceneName, bReload);
		}
	}
}