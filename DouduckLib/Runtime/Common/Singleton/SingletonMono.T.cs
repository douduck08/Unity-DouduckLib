﻿using System;
using UnityEngine;
/// Ref: http://wiki.unity3d.com/index.php/Singleton
namespace DouduckLib {
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        private static GameObject _gameObject;
        private static object _lock = new object ();

        protected SingletonMono () { }

        public static T instance {
            get {
                if (applicationIsQuitting) {
                    return null;
                }

                lock (_lock) {
                    if (_instance == null) {
                        var result = FindObjectsOfType (typeof (T));
                        if (result.Length > 1) {
                            throw new InvalidOperationException ("[Singleton] There should never be more than 1 singleton!");
                        } else if (result.Length == 1) {
                            _instance = (T) result[0];
                        }

                        if (_instance == null) {
                            var go = new GameObject ("[Singleton]" + typeof (T).Name);
                            var component = go.AddComponent<T> ();
                            InitializeSingletonMono (component);
                        }
                    }
                    return _instance;
                }
            }
        }

        protected static void InitializeSingletonMono (T component, bool dontDestroyOnLoad = true) {
            if (_instance != null) {
                if (_instance != component)
                    throw new InvalidOperationException ("[Singleton] There should never be more than 1 singleton!");
                else
                    return;
            }
            _instance = component;
            _gameObject = component.gameObject;
            if (dontDestroyOnLoad) DontDestroyOnLoad (_gameObject);
        }

        private static bool applicationIsQuitting = false;
        protected virtual void Awake () {
            applicationIsQuitting = false;
        }

        protected void OnDestroy () {
            _instance = null;
            applicationIsQuitting = true;
        }
    }
}