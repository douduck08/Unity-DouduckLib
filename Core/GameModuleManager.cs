using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckGame {
    public class GameModuleManager {

        private Dictionary<Type, IModular> m_GameModuleList;
        private List<IModuleUpdatable> m_GameModuleUpdateList;

        public GameModuleManager () {
            m_GameModuleList = new Dictionary<Type, IModular> ();
            m_GameModuleUpdateList = new List<IModuleUpdatable> ();
        }

        public void UpdateModule () {
            for (int i = m_GameModuleUpdateList.Count - 1; i >= 0; i--) {
                m_GameModuleUpdateList[i].ModuleUpdate ();
            }
        }

        public T AddModule<T>() where T : class, IModular, new() {
            if (m_GameModuleList.ContainsKey (typeof (T))) {
                Util.UnityConsole.LogError ("[GameModuleManager] There was a " + typeof (T).Name);
                return null;
            } else {
                T newModule_ = new T();
                newModule_.ModuleInitialize ();
                m_GameModuleList.Add (newModule_.GetType (), newModule_);
                if (newModule_ is IModuleUpdatable) {
                    m_GameModuleUpdateList.Add (newModule_ as IModuleUpdatable);
                }
                return newModule_;
            }
        }

        public void RemoveModule<T>() where T : class, IModular {
            if (m_GameModuleList.ContainsKey (typeof (T))) {
                m_GameModuleList[typeof (T)].ModuleUninitialize ();
                if (m_GameModuleList[typeof (T)] is IModuleUpdatable) {
                    m_GameModuleUpdateList.Remove (m_GameModuleList[typeof (T)] as IModuleUpdatable);
                }
                m_GameModuleList.Remove (typeof (T));
                
            } else {
                Util.UnityConsole.LogError ("[GameModuleManager] There was no " + typeof (T).Name);
            }
        }

        public T GetModule<T>() where T : class, IModular {
            if (m_GameModuleList.ContainsKey (typeof (T))) {
                return m_GameModuleList[typeof (T)] as T;
            } else {
                Util.UnityConsole.LogError ("[GameModuleManager] There was no " + typeof (T).Name);
                return null;
            }
        }
    }
}
