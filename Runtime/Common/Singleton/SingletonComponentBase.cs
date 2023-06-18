using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DouduckLib
{
    public abstract class SingletonComponentBase : MonoBehaviour
    {

        protected void Awake()
        {
            OnSingletonAwakeInternal();
            OnSingletonAwake();
        }

        protected void OnDestroy()
        {
            OnSingletonDestroyInternal();
            OnSingletonDestroy();
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
                DontDestroyOnLoad(gameObject);
                Debug.Log("[Singleton] An instance of " + typeof(T) + " was created as DontDestroyOnLoad.", instance);
            }
            else
            {
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
