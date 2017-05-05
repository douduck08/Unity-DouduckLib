using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DouduckGame.GameFlow;

namespace DouduckGame {
    [DisallowMultipleComponent]
    [AddComponentMenu ("DouduckGame/GameSystemMono/GameFlowSystem")]
    public class GameFlowSystem : GameSystemMono {

        private GameFlowController m_GameFlowController;

        public override void StartGameSystem () {
            m_GameFlowController = new GameFlowController ();
        }

        public override void DestoryGameSystem () {

        }

        void Update () {
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