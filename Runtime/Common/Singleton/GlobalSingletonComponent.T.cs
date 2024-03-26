using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public abstract class GlobalSingletonComponent<T> : SingletonComponentBase where T : SingletonComponentBase
    {
        private static T _instance;
        private static readonly object _lock = new();
        private static bool _autoCreating = false;
        private static bool _applicationIsQuitting = false;

        public static T Get()
        {
            if (_applicationIsQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                if (_instance == null)
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
            _applicationIsQuitting = true;
            _instance = null;
        }
    }
}
