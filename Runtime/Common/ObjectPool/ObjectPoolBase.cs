using System;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    [Serializable]
    public abstract class ObjectPoolBase<TObject>
    {
        [SerializeField] TObject _prefab;
        [SerializeField] int _initialSize;
        [SerializeField] int _maxSize;

        bool _isInitialized = false;
        Stack<TObject> _inactiveObjects;
        HashSet<TObject> _activeObjects;
        List<TObject> _tempActiveList;

        public event Action<TObject> OnCreated;
        public event Action<TObject> OnSpawned;
        public event Action<TObject> OnDespawned;
        public event Action<TObject> OnReleased;

        public int ActiveObjectNumber => _activeObjects?.Count ?? 0;
        public int InactiveObjectNumber => _inactiveObjects?.Count ?? 0;
        public int TotalObjectNumber => ActiveObjectNumber + InactiveObjectNumber;
        public bool IsInitialized() => _isInitialized;

        protected abstract TObject InstantiateObject(TObject prefab);
        protected abstract void ReleaseObject(TObject item);
        protected abstract void SetObjectActive(TObject item, bool active);

        TObject AllocNew(bool active)
        {
            if (_prefab == null)
            {
                throw new InvalidOperationException("Cannot allocate object because prefab is null. Ensure the pool is properly initialized.");
            }
            var item = InstantiateObject(_prefab);
            OnCreated?.Invoke(item);
            SetObjectActive(item, active);
            return item;
        }

        public ObjectPoolBase<TObject> InitializePool()
        {
            if (!_isInitialized)
            {
                if (_prefab == null && _initialSize > 0)
                {
                    throw new InvalidOperationException("Cannot pre-warm pool because prefab is null.");
                }
                _isInitialized = true;
                _inactiveObjects = new Stack<TObject>(_initialSize);
                _activeObjects = new HashSet<TObject>();
                _tempActiveList = new List<TObject>();
                for (int i = 0; i < _initialSize; i++)
                {
                    _inactiveObjects.Push(AllocNew(false));
                }
            }
            return this;
        }

        public ObjectPoolBase<TObject> InitializePool(TObject prefab, int initialSize)
        {
            return InitializePool(prefab, initialSize, 0);
        }

        public ObjectPoolBase<TObject> InitializePool(TObject prefab, int initialSize, int maxSize)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            _maxSize = maxSize;
            return InitializePool();
        }

        public TObject Spawn()
        {
            if (!_isInitialized)
            {
                InitializePool();
            }

            TObject item;
            if (_inactiveObjects.Count == 0)
            {
                item = AllocNew(true);
            }
            else
            {
                item = _inactiveObjects.Pop();
                SetObjectActive(item, true);
            }

            _activeObjects.Add(item);
            OnSpawned?.Invoke(item);
            return item;
        }

        public void Despawn(TObject item)
        {
            if (item == null)
            {
                return;
            }

            if (!_activeObjects.Remove(item))
            {
                Debug.LogWarning($"[ObjectPool] Attempted to despawn an object ({item}) that is not active or does not belong to this pool.");
                return;
            }

            SetObjectActive(item, false);
            OnDespawned?.Invoke(item);

            if (_maxSize > 0 && _inactiveObjects.Count >= _maxSize)
            {
                OnReleased?.Invoke(item);
                ReleaseObject(item);
            }
            else
            {
                _inactiveObjects.Push(item);
            }
        }

        public void DespawnAll()
        {
            if (_activeObjects == null || _activeObjects.Count == 0)
            {
                return;
            }

            _tempActiveList.Clear();
            _tempActiveList.AddRange(_activeObjects);

            for (int i = 0; i < _tempActiveList.Count; i++)
            {
                Despawn(_tempActiveList[i]);
            }
            _tempActiveList.Clear();
        }

        public void ReleasePool()
        {
            if (_isInitialized)
            {
                DespawnAll();

                while (_inactiveObjects.Count > 0)
                {
                    var item = _inactiveObjects.Pop();
                    OnReleased?.Invoke(item);
                    ReleaseObject(item);
                }

                _activeObjects.Clear();
                _inactiveObjects.Clear();
                _tempActiveList.Clear();
                _isInitialized = false;
            }
        }
    }
}
