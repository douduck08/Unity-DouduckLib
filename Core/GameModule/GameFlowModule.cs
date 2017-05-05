using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DouduckGame.GameFlow;

namespace DouduckGame {
    public class GameFlowModule : IModular, IModuleUpdatable {
        private GameFlowController m_GameFlowController;

        public void ModuleInitialize () {
            m_GameFlowController = new GameFlowController ();
        }

        public void ModuleUninitialize () {

        }

        public void ModuleUpdate () {
            m_GameFlowController.StateUpdate ();
        }

        public void Terminate () {
            m_GameFlowController.Terminate ();
            m_GameFlowController = new GameFlowController ();
        }

        public GameFlowController TransTo (State oState, int iLevel = 0) {
            return m_GameFlowController.TransTo (oState, iLevel);
        }

        public GameFlowController LoadScene (string sSceneName) {
            return m_GameFlowController.LoadScene (sSceneName);
        }
    }
}