using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DouduckLib
{
    public abstract class LocalSingletonComponent<T> : SingletonComponentBase where T : SingletonComponentBase
    {
        private static readonly Dictionary<int, T> _loacalInstance = new();
        private static readonly object _lock = new();
        private static bool _autoCreating = false;
        private static bool _applicationIsQuitting = false;

        public static T Get(GameObject gameObject)
        {
            if (_applicationIsQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                var sceneHandle = gameObject.scene.handle;
                if (!_loacalInstance.ContainsKey(sceneHandle))
                {
                    _autoCreating = true;
                    _loacalInstance.Add(sceneHandle, MoveToScene(CreateInstance<T>(false), gameObject.scene));
                    _autoCreating = false;
                }
                return _loacalInstance[sceneHandle];
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
            if (_autoCreating)
            {
                return;
            }
            var sceneHandle = gameObject.scene.handle;
            if (!_loacalInstance.ContainsKey(sceneHandle))
            {
                _loacalInstance.Add(sceneHandle, this as T);
                Debug.Log("[Singleton] An instance of " + typeof(T) + " has became singleton.", this);
            }
            else if (_loacalInstance[sceneHandle] != this)
            {
                throw new System.InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T) + " in the same scene.");
            }
        }

        protected sealed override void OnSingletonDestroyInternal()
        {
            var sceneHandle = gameObject.scene.handle;
            _loacalInstance.Remove(sceneHandle);
        }

        protected void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }
}
