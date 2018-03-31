using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public interface IObjectPool {
        int totalCount { get; }
        int activeCount { get; }
        int inactiveCount { get; }
    }

    public interface IObjectPool<TObject> : IObjectPool {
        TObject Spawn ();
        void Despawn (TObject item);
    }

    public interface IObjectPool<TObject, in TParam1> : IObjectPool {
        TObject Spawn (TParam1 param1);
        void Despawn (TObject item);
    }

    public interface IObjectPool<TObject, in TParam1, in TParam2> : IObjectPool {
        TObject Spawn (TParam1 param1, TParam2 param2);
        void Despawn (TObject item);
    }

    public interface IObjectPool<TObject, in TParam1, in TParam2, in TParam3> : IObjectPool {
        TObject Spawn (TParam1 param1, TParam2 param2, TParam3 param3);
        void Despawn (TObject item);
    }
}