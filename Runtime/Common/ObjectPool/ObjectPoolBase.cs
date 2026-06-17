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
        [SerializeField] int _maxSize;

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
            return InitializePool(prefab, initialSize, 0);
        }

        public ObjectPoolBase<TObject, TData> InitializePool(TObject prefab, int initialSize, int maxSize)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            _maxSize = maxSize;
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

            if (_maxSize > 0 && _inactiveObjects.Count >= _maxSize)
            {
                _onReleased?.Invoke(item);
                ReleaseObject(item);
            }
            else
            {
                _inactiveObjects.Push(item);
            }
        }

        public void DespawnAll()
        {
            while (_activeObjects.Count > 0)
            {
                var enumerator = _activeObjects.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    enumerator.Dispose();
                    Despawn(item);
                }
                else
                {
                    enumerator.Dispose();
                    break;
                }
            }
        }

        public void ReleasePool()
        {
            if (_isInitialized)
            {
                DespawnAll();

                while (_inactiveObjects.Count > 0)
                {
                    var item = _inactiveObjects.Pop();
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
