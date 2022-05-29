using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public interface IPooledComponent<TInitParam, TSpawnParam> {
        void OnCreateFromPool (TInitParam initParam);
        void OnSpawnFromPool (TSpawnParam spawnParam);
        void OnReturnToPool ();
    }

    public class GameObjectPool<TObject, TInitParam, TSpawnParam> where TObject : Component, IPooledComponent<TInitParam, TSpawnParam> {

        public TObject prefab;
        public int initialSize;
        public Transform instantiatePatent;
        public TInitParam initialParam;

        Stack<TObject> inactiveObjects;
        HashSet<TObject> activeObjects;

        public int activeCount {
            get {
                return activeObjects.Count;
            }
        }
        public int inactiveCount {
            get {
                return inactiveObjects.Count;
            }
        }
        public int totalCount {
            get {
                return activeCount + inactiveCount;
            }
        }

        public void InitializePool () {
            inactiveObjects = new Stack<TObject> (initialSize);
            activeObjects = new HashSet<TObject> ();
            for (int i = 0; i < initialSize; i++) {
                inactiveObjects.Push (AllocNew (false));
            }
        }

        TObject AllocNew (bool active) {
            var item = GameObject.Instantiate<TObject> (prefab, instantiatePatent);
            item.OnCreateFromPool (initialParam);
            item.gameObject.SetActive (active);
            return item;
        }

        public TObject Spawn (TSpawnParam spawnParam) {
            if (inactiveObjects.Count == 0) {
                var item = AllocNew (true);
                item.OnSpawnFromPool (spawnParam);
                activeObjects.Add (item);
                return item;
            } else {
                var item = inactiveObjects.Pop ();
                item.gameObject.SetActive (true);
                item.OnSpawnFromPool (spawnParam);
                activeObjects.Add (item);
                return item;
            }
        }

        public void Despawn (TObject item) {
            item.gameObject.SetActive (false);
            item.OnReturnToPool ();
            activeObjects.Remove (item);
            inactiveObjects.Push (item);
        }
    }
}