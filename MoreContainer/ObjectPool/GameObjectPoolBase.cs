using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public enum PoolExpandMethods {
        Fixed,
        OneAtATime,
        Double
    }

    [System.Serializable]
    public struct ObjectPoolSettings {
        public int initialSize;
        public PoolExpandMethods expandMethod;
    }

    public abstract class GameObjectPoolBase<TObject> : IObjectPool where TObject : Component {
        ObjectPoolSettings m_settings;
        TObject m_prefab;
        Stack<TObject> m_inactiveObjects;
        HashSet<TObject> m_activeObjects;

        public int activeCount {
            get {
                return m_activeObjects.Count;
            }
        }
        public int inactiveCount {
            get {
                return m_inactiveObjects.Count;
            }
        }
        public int totalCount {
            get {
                return activeCount + inactiveCount;
            }
        }

        protected void Initialize (ObjectPoolSettings settings, TObject prefab) {
            m_settings = settings;
            m_prefab = prefab;
            m_inactiveObjects = new Stack<TObject>(m_settings.initialSize);
            m_activeObjects = new HashSet<TObject>();
            for (int i = 0; i < m_settings.initialSize; i++) {
                m_inactiveObjects.Push(AllocNew());
            }
        }

        TObject AllocNew () {
            var item = GameObject.Instantiate<TObject> (m_prefab);
            item.gameObject.SetActive (false);
            OnCreated (item);
            return item;
        }

        void ExpandPool () {
            switch (m_settings.expandMethod) {
                case PoolExpandMethods.Fixed:
                    throw new System.InvalidOperationException ("[ObjectPool] Exceeded max size of pool");
                case PoolExpandMethods.OneAtATime:
                    m_inactiveObjects.Push(AllocNew());
                    break;
                case PoolExpandMethods.Double:
                    if (totalCount == 0) {
                        m_inactiveObjects.Push(AllocNew());
                    } else {
                        var oldSize = totalCount;
                        if (oldSize == 0) {
                            oldSize = 1;
                        }
                        for (int i = 0; i < oldSize; i++) {
                            m_inactiveObjects.Push(AllocNew());
                        }
                    }
                    break;
                default:
                    throw new System.InvalidOperationException ("[ObjectPool] Unkowned expandMethod");
            }
        } 

        protected TObject GetInternal() {
            if (m_inactiveObjects.Count == 0) {
                ExpandPool();
            }
            TObject item = m_inactiveObjects.Pop();
            m_activeObjects.Add(item);
            item.gameObject.SetActive (true);
            OnSpawned (item);
            return item;
        }

        public void Despawn(TObject item) {
            if (m_inactiveObjects.Contains(item)) {
                throw new System.InvalidOperationException ("[Pool] Despawn twince");
            }
            m_activeObjects.Remove(item);
            m_inactiveObjects.Push(item);
            OnDespawned (item);
        }

        public void DespawnAll () {
            foreach (TObject item in m_activeObjects) {
                m_inactiveObjects.Push(item);
                OnDespawned (item);
            }
            m_activeObjects.Clear ();
        }

        public void Release (bool includeActive = false) {
            while (m_inactiveObjects.Count > 0) {
                TObject item = m_inactiveObjects.Pop();
                GameObject.Destroy (item.gameObject);
            } 

            if (includeActive) {
                foreach (TObject item in m_activeObjects) {
                    GameObject.Destroy (item.gameObject);
                }
                m_activeObjects.Clear ();
            }
        }

        protected virtual void OnCreated (TObject item) {
            item.gameObject.SetActive (false);
        }

        protected virtual void OnSpawned (TObject item) {
            item.gameObject.SetActive (true);
        }

        protected virtual void OnDespawned(TObject item) {
            item.gameObject.SetActive (true);
        }
    }
}