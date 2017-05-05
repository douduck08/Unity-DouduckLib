using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public class GameObjectController {

        private List<GameObjectSet> m_GameObjectSetList;

        public void AddGameObjectSet (GameObjectSet set) {
            m_GameObjectSetList.Add (set);
        }

        public void AddGameObjectSet (GameObject gameObject) {
            GameObjectSet set = gameObject.GetComponent<GameObjectSet> ();
            if (set != null) {
                m_GameObjectSetList.Add (set);
            } else {
                Util.UnityConsole.LogError ("[GameObjectController] Add failure");
            }
        }

        public T GetGameObjectSet<T>(string setName) where T : GameObjectSet {
            GameObjectSet set_ = m_GameObjectSetList.Find (p => p.Name == setName);
            if (set_ == null) {
                Util.UnityConsole.LogError ("[GameObjectController] Find no set named " + setName);
                return null;
            } else {
                return set_ as T;
            }
        }

        public void Show (string setName) {
            GameObjectSet set_ = m_GameObjectSetList.Find (p => p.Name == setName);
            if (set_ == null) {
                Util.UnityConsole.LogError ("[GameObjectController] Find no set named " + setName);
            } else {
                set_.Show ();
            }
        }

        public void Hide (string setName) {
            GameObjectSet set_ = m_GameObjectSetList.Find (p => p.Name == setName);
            if (set_ == null) {
                Util.UnityConsole.LogError ("[GameObjectController] Find no set named " + setName);
            } else {
                set_.Hide ();
            }
        }
    }
}