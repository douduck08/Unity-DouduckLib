using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public abstract class SingletonComponentBase : MonoBehaviour
    {
        protected static bool dontDestroyOnLoad => dontDestroyOnLoad_;
        protected static bool applicationIsQuitting => applicationIsQuitting_;
        private static bool dontDestroyOnLoad_ = false;
        private static bool applicationIsQuitting_ = false;

        protected void Awake()
        {
            OnSingletonAwake();
        }

        protected void OnDestroy()
        {
            if (dontDestroyOnLoad_)
            {
                applicationIsQuitting_ = true;
            }
            else
            {
                OnSingletonDestroy();
            }
        }

        protected void OnApplicationQuit()
        {
            applicationIsQuitting_ = true;
        }

        protected virtual void OnSingletonAwake() { }
        protected virtual void OnSingletonDestroy() { }

        protected static T CreateInstance<T>(bool scrossScene) where T : SingletonComponentBase
        {
            var gameObject = new GameObject() { name = typeof(T).Name + " (Singleton)" };
            var instance = gameObject.AddComponent<T>();
            if (scrossScene)
            {
                dontDestroyOnLoad_ = true;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[Singleton] An instance of " + typeof(T) + " was created as DontDestroyOnLoad.");
            }
            else
            {
                dontDestroyOnLoad_ = false;
                Debug.Log("[Singleton] An instance of " + typeof(T) + " was created.");
            }
            return instance;
        }
    }
}
