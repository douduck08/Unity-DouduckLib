using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            OnSingletonAwakeInternal();
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
                OnSingletonDestroyInternal();
                OnSingletonDestroy();
            }
        }

        protected void OnApplicationQuit()
        {
            applicationIsQuitting_ = true;
        }

        protected abstract void OnSingletonAwakeInternal();
        protected abstract void OnSingletonDestroyInternal();
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
                Debug.Log("[Singleton] An instance of " + typeof(T) + " was created as DontDestroyOnLoad.", instance);
            }
            else
            {
                dontDestroyOnLoad_ = false;
                Debug.Log("[Singleton] An instance of " + typeof(T) + " was created.", instance);
            }
            return instance;
        }

        protected static T MoveToScene<T>(T instance, Scene scene) where T : SingletonComponentBase
        {
            if (instance.gameObject.scene.handle != scene.handle)
            {
                SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
            }
            return instance;
        }
    }
}
