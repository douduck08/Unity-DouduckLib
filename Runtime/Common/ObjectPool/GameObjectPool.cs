using System;
using UnityEngine;

namespace DouduckLib
{
    [Serializable]
    public class GameObjectPool<TObject> : ObjectPoolBase<TObject> where TObject : Component
    {
        [SerializeField] Transform _instantiateParent;

        public new GameObjectPool<TObject> InitializePool(TObject prefab, int initialSize)
        {
            base.InitializePool(prefab, initialSize, 0);
            return this;
        }

        public new GameObjectPool<TObject> InitializePool(TObject prefab, int initialSize, int maxSize)
        {
            base.InitializePool(prefab, initialSize, maxSize);
            return this;
        }

        public GameObjectPool<TObject> InitializePool(TObject prefab, Transform instantiateParent, int initialSize)
        {
            return InitializePool(prefab, instantiateParent, initialSize, 0);
        }

        public GameObjectPool<TObject> InitializePool(TObject prefab, Transform instantiateParent, int initialSize, int maxSize)
        {
            _instantiateParent = instantiateParent;
            base.InitializePool(prefab, initialSize, maxSize);
            return this;
        }

        protected override TObject InstantiateObject(TObject prefab)
        {
            return GameObject.Instantiate<TObject>(prefab, _instantiateParent);
        }

        protected override void ReleaseObject(TObject item)
        {
            if (item != null && item.gameObject != null)
            {
                GameObject.Destroy(item.gameObject);
            }
        }

        protected override void SetObjectActive(TObject item, bool active)
        {
            if (item != null && item.gameObject != null)
            {
                item.gameObject.SetActive(active);
            }
        }
    }

    [Serializable]
    public class GameObjectPool<TObject, TData> : GameObjectPool<TObject> where TObject : Component
    {
        public new event Action<TObject, TData> OnSpawned;

        public new GameObjectPool<TObject, TData> InitializePool(TObject prefab, int initialSize)
        {
            base.InitializePool(prefab, initialSize, 0);
            return this;
        }

        public new GameObjectPool<TObject, TData> InitializePool(TObject prefab, int initialSize, int maxSize)
        {
            base.InitializePool(prefab, initialSize, maxSize);
            return this;
        }

        public new GameObjectPool<TObject, TData> InitializePool(TObject prefab, Transform instantiateParent, int initialSize)
        {
            base.InitializePool(prefab, instantiateParent, initialSize, 0);
            return this;
        }

        public new GameObjectPool<TObject, TData> InitializePool(TObject prefab, Transform instantiateParent, int initialSize, int maxSize)
        {
            base.InitializePool(prefab, instantiateParent, initialSize, maxSize);
            return this;
        }

        public TObject Spawn(TData spawnParam)
        {
            var item = base.Spawn();
            OnSpawned?.Invoke(item, spawnParam);
            return item;
        }
    }
}
