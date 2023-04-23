using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class PooledComponent<TObject, TInitParam, TSpawnParam> : MonoBehaviour, IPooledComponent<TInitParam, TSpawnParam>
    {
        public virtual void OnCreateFromPool(TInitParam initParam) { }
        public virtual void OnSpawnFromPool(TSpawnParam spawnParam) { }
        public virtual void OnReturnToPool() { }
    }
}
