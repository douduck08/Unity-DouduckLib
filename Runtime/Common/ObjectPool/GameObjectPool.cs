using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class GameObjectPool<TObject, TData> : ObjectPoolBase<TObject, TData> where TObject : Component
    {
        [SerializeField] Transform instantiatePatent;

        public ObjectPoolBase<TObject, TData> InitializePool(TObject prefab, Transform instantiatePatent, int initialSize)
        {
            this.instantiatePatent = instantiatePatent;
            return InitializePool(prefab, initialSize);
        }

        protected override TObject InstantiateObject(TObject prefab)
        {
            return GameObject.Instantiate<TObject>(prefab, instantiatePatent);
        }

        protected override void ReleaseObject(TObject item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void SetObjectActive(TObject item, bool active)
        {
            item.gameObject.SetActive(active);
        }
    }
}
