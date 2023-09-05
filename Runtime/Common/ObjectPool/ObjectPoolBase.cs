using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DouduckLib
{
    public abstract class ObjectPoolBase<TObject, TData>
    {
        [SerializeField] TObject prefab;
        [SerializeField] int initialSize;

        bool isInitialized = false;
        Stack<TObject> inactiveObjects;
        HashSet<TObject> activeObjects;

        Action<TObject> onCreated;
        Action<TObject, TData> onSpawned;
        Action<TObject> onDespawned;
        Action<TObject> onReleased;

        public int activeObjectNumber => activeObjects.Count;
        public int inactiveObjectNumber => inactiveObjects.Count;
        public int totalObjectNumber => activeObjects.Count + inactiveObjects.Count;
        public bool IsInitialized() => isInitialized;

        protected abstract TObject InstantiateObject(TObject prefab);
        protected abstract void ReleaseObject(TObject item);
        protected abstract void SetObjectActive(TObject item, bool active);

        TObject AllocNew(bool active)
        {
            var item = InstantiateObject(prefab);
            onCreated?.Invoke(item);
            SetObjectActive(item, active);
            return item;
        }

        public ObjectPoolBase<TObject, TData> InitializePool()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                inactiveObjects = new Stack<TObject>(initialSize);
                activeObjects = new HashSet<TObject>();
                for (int i = 0; i < initialSize; i++)
                {
                    inactiveObjects.Push(AllocNew(false));
                }
            }
            return this;
        }

        public ObjectPoolBase<TObject, TData> InitializePool(TObject prefab, int initialSize)
        {
            this.prefab = prefab;
            this.initialSize = initialSize;
            return InitializePool();
        }

        public ObjectPoolBase<TObject, TData> OnCreated(Action<TObject> onCreatedCallback)
        {
            onCreated = onCreatedCallback;
            return this;
        }

        public ObjectPoolBase<TObject, TData> OnSpawned(Action<TObject, TData> onSpawnedCallback)
        {
            onSpawned = onSpawnedCallback;
            return this;
        }

        public ObjectPoolBase<TObject, TData> OnDespawned(Action<TObject> onDespawnedCallback)
        {
            onDespawned = onDespawnedCallback;
            return this;
        }

        public ObjectPoolBase<TObject, TData> OnReleased(Action<TObject> onReleasedCallback)
        {
            onReleased = onReleasedCallback;
            return this;
        }

        public TObject Spawn(TData spawnParam)
        {
            InitializePool();
            if (inactiveObjects.Count == 0)
            {
                var item = AllocNew(true);
                onSpawned?.Invoke(item, spawnParam);
                activeObjects.Add(item);
                return item;
            }
            else
            {
                var item = inactiveObjects.Pop();
                SetObjectActive(item, true);
                onSpawned?.Invoke(item, spawnParam);
                activeObjects.Add(item);
                return item;
            }
        }

        public void Despawn(TObject item)
        {
            SetObjectActive(item, false);
            onDespawned?.Invoke(item);
            activeObjects.Remove(item);
            inactiveObjects.Push(item);
        }

        public void DespawnAll()
        {
            var objectCache = activeObjects.ToList();
            foreach (var item in objectCache)
            {
                Despawn(item);
            }
        }

        public void ReleasePool()
        {
            if (isInitialized)
            {
                var objectCache = activeObjects.ToList();
                foreach (var item in objectCache)
                {
                    Despawn(item);
                }

                objectCache = inactiveObjects.ToList();
                foreach (var item in objectCache)
                {
                    onReleased?.Invoke(item);
                    ReleaseObject(item);
                }

                activeObjects.Clear();
                inactiveObjects.Clear();
                isInitialized = false;
            }
        }
    }
}
