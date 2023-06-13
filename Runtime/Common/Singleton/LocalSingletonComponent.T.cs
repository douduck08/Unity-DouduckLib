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
        private static bool autoCreating = false;

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
                    autoCreating = true;
                    loacalInstance_.Add(sceneHandle, MoveToScene(CreateInstance<T>(false), gameObject.scene));
                    autoCreating = false;
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

        protected sealed override void OnSingletonAwakeInternal()
        {
            if (autoCreating)
            {
                return;
            }
            var sceneHandle = gameObject.scene.handle;
            if (!loacalInstance_.ContainsKey(sceneHandle))
            {
                loacalInstance_.Add(sceneHandle, this as T);
            }
            else if (loacalInstance_[sceneHandle] != this)
            {
                throw new System.InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T) + " in the same scene.");
            }
        }

        protected sealed override void OnSingletonDestroyInternal()
        {
            var sceneHandle = gameObject.scene.handle;
            loacalInstance_.Remove(sceneHandle);
        }
    }
}
