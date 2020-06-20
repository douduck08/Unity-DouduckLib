using System;
using UnityEngine;

namespace DouduckLib {
    public abstract class SingletonMonoAuto<T> : SingletonMonoBase where T : SingletonMonoBase {
        private static T _instance;
        private static GameObject _gameObject;
        private static object _lock = new object ();
        private static bool _dontDestroyOnLoad = false;
        private static bool _applicationIsQuitting = false;

        public static T instance {
            get {
                if (_applicationIsQuitting) {
                    return null;
                }

                lock (_lock) {
                    if (_instance == null) {
                        _gameObject = new GameObject () { name = typeof (T).Name + " (Singleton)" };
                        _instance = _gameObject.AddComponent<T> ();
                        _instance.OnSingletonAwake ();
                    }
                    return _instance;
                }
            }
        }

        protected void MarkAsCrossSceneSingleton () {
            _dontDestroyOnLoad = true;
            DontDestroyOnLoad (_gameObject);
            Debug.Log ("[Singleton] An instance of " + typeof (T) + "was marked as DontDestroyOnLoad.");
        }

        protected void Awake () { }

        protected void OnDestroy () {
            if (_dontDestroyOnLoad) {
                _applicationIsQuitting = true;
            } else {
                _instance = null;
                _gameObject = null;
            }
            OnSingletonDestroy ();
        }

        protected void OnApplicationQuit () {
            _applicationIsQuitting = true;
        }
    }
}