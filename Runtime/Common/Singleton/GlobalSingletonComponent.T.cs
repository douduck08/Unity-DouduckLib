using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public abstract class GlobalSingletonComponent<T> : SingletonComponentBase where T : SingletonComponentBase
    {
        private static T instance_;
        private static readonly object lock_ = new();

        public static T Get()
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            lock (lock_)
            {
                if (instance_ == null)
                {
                    instance_ = CreateInstance<T>(true);
                }
                return instance_;
            }
        }

        protected sealed override void OnSingletonAwakeInternal()
        {
            if (instance_ == null)
            {
                lock (lock_)
                {
                    instance_ = this as T;
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (instance_ != this)
            {
                throw new System.InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T));
            }
        }

        protected sealed override void OnSingletonDestroyInternal()
        {
            instance_ = null;
        }
    }
}
