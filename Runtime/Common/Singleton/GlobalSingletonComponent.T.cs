using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public abstract class GlobalSingletonComponent<T> : GlobalSingletonComponent<T, SingletonOption.AutoCreate> where T : SingletonComponentBase { }

    public abstract class GlobalSingletonComponent<T, S> : SingletonComponentBase where T : SingletonComponentBase where S : SingletonOption.IOption
    {
        static T _instance;
        static readonly object _lock = new();
        static bool _autoCreating = false;
        static bool _applicationIsQuitting = false;

        public static T Get()
        {
            if (_applicationIsQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                if (_instance == null && typeof(S) == typeof(SingletonOption.AutoCreate))
                {
                    _autoCreating = true;
                    _instance = CreateInstance<T>(true);
                    _autoCreating = false;
                }
                return _instance;
            }
        }

        protected sealed override void OnSingletonAwakeInternal()
        {
            if (_autoCreating)
            {
                return;
            }
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[Singleton] An instance of " + typeof(T) + " has became singleton as DontDestroyOnLoad.", this);
            }
            else if (_instance != this)
            {
                throw new System.InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T));
            }
        }

        protected sealed override void OnSingletonDestroyInternal()
        {
            _instance = null;
        }

        protected void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }
}
