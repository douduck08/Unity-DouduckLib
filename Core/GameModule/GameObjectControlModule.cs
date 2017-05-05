using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public class GameObjectControlModule : IModular {

        private GameObjectController m_controller;

        public void ModuleInitialize () {
            m_controller = new GameObjectController ();
        }

        public void ModuleUninitialize () {
        }

        public void AddGameObjectSet (GameObjectSet set) {
            m_controller.AddGameObjectSet (set);
        }

        public void AddGameObjectSet (GameObject gameObject) {
            m_controller.AddGameObjectSet (gameObject);
        }

        public T GetGameObjectSet<T>(string setName) where T : GameObjectSet {
            return m_controller.GetGameObjectSet<T> (setName);
        }

        public void Show (string setName) {
            m_controller.Show (setName);
        }

        public void Hide (string setName) {
            m_controller.Show (setName);
        }
    }
}