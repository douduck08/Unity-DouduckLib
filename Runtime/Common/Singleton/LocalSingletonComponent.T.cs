using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DouduckLib
{
    public abstract class LocalSingletonComponent<T> : LocalSingletonComponent<T, SingletonOption.AutoCreate> where T : SingletonComponentBase { }

    public abstract class LocalSingletonComponent<T, S> : SingletonComponentBase where T : SingletonComponentBase where S : SingletonOption.IOption
    {
        static readonly Dictionary<int, T> _localInstance = new();
        static readonly object _lock = new();
        static bool _autoCreating = false;
        static bool _applicationIsQuitting = false;

        public static T Get(GameObject gameObject)
        {
            if (_applicationIsQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                var sceneHandle = gameObject.scene.handle;
                if (!_localInstance.ContainsKey(sceneHandle) && typeof(S) == typeof(SingletonOption.AutoCreate))
                {
                    _autoCreating = true;
                    _localInstance.Add(sceneHandle, MoveToScene(CreateInstance<T>(false), gameObject.scene));
                    _autoCreating = false;
                }
                if (_localInstance.ContainsKey(sceneHandle))
                {
                    return _localInstance[sceneHandle];
                }
                return null;
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
            if (!_localInstance.ContainsKey(sceneHandle))
            {
                _localInstance.Add(sceneHandle, this as T);
                Debug.Log("[Singleton] An instance of " + typeof(T) + " has became singleton.", this);
            }
            else if (_localInstance[sceneHandle] != this)
            {
                throw new System.InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T) + " in the same scene.");
            }
        }

        protected sealed override void OnSingletonDestroyInternal()
        {
            var sceneHandle = gameObject.scene.handle;
            _localInstance.Remove(sceneHandle);
        }

        protected void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }
}
