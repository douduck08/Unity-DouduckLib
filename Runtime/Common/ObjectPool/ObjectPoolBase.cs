using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DouduckLib
{
    public abstract class ObjectPoolBase<TObject, TData>
    {
        [SerializeField] TObject _prefab;
        [SerializeField] int _initialSize;

        bool _isInitialized = false;
        Stack<TObject> _inactiveObjects;
        HashSet<TObject> _activeObjects;

        Action<TObject> _onCreated;
        Action<TObject, TData> _onSpawned;
        Action<TObject> _onDespawned;
        Action<TObject> _onReleased;

        public int ActiveObjectNumber => _activeObjects.Count;
        public int InactiveObjectNumber => _inactiveObjects.Count;
        public int TotalObjectNumber => _activeObjects.Count + _inactiveObjects.Count;
        public bool IsInitialized() => _isInitialized;

        protected abstract TObject InstantiateObject(TObject prefab);
        protected abstract void ReleaseObject(TObject item);
        protected abstract void SetObjectActive(TObject item, bool active);

        TObject AllocNew(bool active)
        {
            var item = InstantiateObject(_prefab);
            _onCreated?.Invoke(item);
            SetObjectActive(item, active);
            return item;
        }

        public ObjectPoolBase<TObject, TData> InitializePool()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _inactiveObjects = new Stack<TObject>(_initialSize);
                _activeObjects = new HashSet<TObject>();
                for (int i = 0; i < _initialSize; i++)
                {
                    _inactiveObjects.Push(AllocNew(false));
                }
            }
            return this;
        }

        public ObjectPoolBase<TObject, TData> InitializePool(TObject prefab, int initialSize)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            return InitializePool();
        }

        public ObjectPoolBase<TObject, TData> OnCreated(Action<TObject> onCreatedCallback)
        {
            _onCreated = onCreatedCallback;
            return this;
        }

        public ObjectPoolBase<TObject, TData> OnSpawned(Action<TObject, TData> onSpawnedCallback)
        {
            _onSpawned = onSpawnedCallback;
            return this;
        }

        public ObjectPoolBase<TObject, TData> OnDespawned(Action<TObject> onDespawnedCallback)
        {
            _onDespawned = onDespawnedCallback;
            return this;
        }

        public ObjectPoolBase<TObject, TData> OnReleased(Action<TObject> onReleasedCallback)
        {
            _onReleased = onReleasedCallback;
            return this;
        }

        public TObject Spawn(TData spawnParam)
        {
            InitializePool();
            if (_inactiveObjects.Count == 0)
            {
                var item = AllocNew(true);
                _onSpawned?.Invoke(item, spawnParam);
                _activeObjects.Add(item);
                return item;
            }
            else
            {
                var item = _inactiveObjects.Pop();
                SetObjectActive(item, true);
                _onSpawned?.Invoke(item, spawnParam);
                _activeObjects.Add(item);
                return item;
            }
        }

        public void Despawn(TObject item)
        {
            SetObjectActive(item, false);
            _onDespawned?.Invoke(item);
            _activeObjects.Remove(item);
            _inactiveObjects.Push(item);
        }

        public void DespawnAll()
        {
            var objectCache = _activeObjects.ToList();
            foreach (var item in objectCache)
            {
                Despawn(item);
            }
        }

        public void ReleasePool()
        {
            if (_isInitialized)
            {
                var objectCache = _activeObjects.ToList();
                foreach (var item in objectCache)
                {
                    Despawn(item);
                }

                objectCache = _inactiveObjects.ToList();
                foreach (var item in objectCache)
                {
                    _onReleased?.Invoke(item);
                    ReleaseObject(item);
                }

                _activeObjects.Clear();
                _inactiveObjects.Clear();
                _isInitialized = false;
            }
        }
    }
}
