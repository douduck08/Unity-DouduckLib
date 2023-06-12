using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DouduckLib
{
    public abstract class LocalSingletonComponent<T> : SingletonComponentBase where T : SingletonComponentBase
    {
        private static readonly Dictionary<int, T> loacalInstance_ = new();
        private static readonly object lock_ = new();

        public static T Get(GameObject gameObject)
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            lock (lock_)
            {
                var sceneHandle = gameObject.scene.handle;
                if (!loacalInstance_.ContainsKey(sceneHandle))
                {
                    var instance = CreateInstance<T>(false);
                    SceneManager.MoveGameObjectToScene(instance.gameObject, gameObject.scene);
                    loacalInstance_.Add(sceneHandle, instance);
                }
                return loacalInstance_[sceneHandle];
            }
        }

        public static T Get(Component component)
        {
            if (component == null)
            {
                return null;
            }
            return Get(component.gameObject);
        }

        protected sealed override void OnSingletonAwake()
        {
            var sceneHandle = gameObject.scene.handle;
            if (!loacalInstance_.ContainsKey(sceneHandle))
            {
                lock (lock_)
                {
                    var newInstance = CreateInstance<T>(false);
                    loacalInstance_.Add(sceneHandle, newInstance);
                }
            }
            else if (loacalInstance_[sceneHandle] != this)
            {
                throw new System.InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T) + " in the same scene.");
            }
        }

        protected sealed override void OnSingletonDestroy()
        {
            var sceneHandle = gameObject.scene.handle;
            loacalInstance_.Remove(sceneHandle);
        }
    }
}
