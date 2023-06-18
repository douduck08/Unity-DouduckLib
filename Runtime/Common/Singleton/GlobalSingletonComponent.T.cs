using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public abstract class GlobalSingletonComponent<T> : SingletonComponentBase where T : SingletonComponentBase
    {
        private static T instance_;
        private static readonly object lock_ = new();
        private static bool autoCreating = false;
        private static bool applicationIsQuitting_ = false;

        public static T Get()
        {
            if (applicationIsQuitting_)
            {
                return null;
            }
            lock (lock_)
            {
                if (instance_ == null)
                {
                    autoCreating = true;
                    instance_ = CreateInstance<T>(true);
                    autoCreating = false;
                }
                return instance_;
            }
        }

        protected sealed override void OnSingletonAwakeInternal()
        {
            if (autoCreating)
            {
                return;
            }
            if (instance_ == null)
            {
                instance_ = this as T;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[Singleton] An instance of " + typeof(T) + " has became singleton as DontDestroyOnLoad.", this);
            }
            else if (instance_ != this)
            {
                throw new System.InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T));
            }
        }

        protected sealed override void OnSingletonDestroyInternal()
        {
            applicationIsQuitting_ = true;
            instance_ = null;
        }
    }
}
