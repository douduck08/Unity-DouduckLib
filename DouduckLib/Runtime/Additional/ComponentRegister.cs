using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public abstract class ComponentRegister<T> : MonoBehaviour where T : MonoBehaviour {
        class CompomentData {
            public string name;
            public string sceneName;
            public string scenePath;
            public MonoBehaviour component;
        }

        private static object _lock = new object ();
        private static List<CompomentData> _componentList = new List<CompomentData> ();

        public static int Count {
            get {
                return _componentList.Count;
            }
        }

        static void InternalAddComponent (MonoBehaviour component) {
            lock (_lock) {
                _componentList.Add (new CompomentData () {
                    component = component,
                        name = component.name,
                        sceneName = component.gameObject.scene.name,
                        scenePath = component.gameObject.scene.path
                });
            }
        }

        static void InternalRemoveComponent (MonoBehaviour compoment) {
            lock (_lock) {
                var index = _componentList.FindIndex (p => p.component == compoment);
                if (index > -1) {
                    _componentList.RemoveAt (index);
                } else {
                    throw new System.InvalidOperationException ("Find no matched component");
                }
            }
        }

        public static T GetComponentFromSceneName (string sceneName) {
            var data = _componentList.Find (p => p.sceneName == sceneName);
            if (data != null) {
                return data.component as T;
            }
            return null;
        }

        public static T GetComponentFromScenePath (string scenePath) {
            var data = _componentList.Find (p => p.scenePath == scenePath);
            if (data != null) {
                return data.component as T;
            }
            return null;
        }

        public static T GetComponentFromName (string name) {
            var data = _componentList.Find (p => p.name == name);
            if (data != null) {
                return data.component as T;
            }
            return null;
        }

        public static T GetComponent (string sceneName, string name) {
            var data = _componentList.Find (p => p.sceneName == sceneName && p.sceneName == sceneName);
            if (data != null) {
                return data.component as T;
            }
            return null;
        }

        protected void Awake () {
            InternalAddComponent (this);
        }

        protected void OnDestroy () {
            InternalRemoveComponent (this);
        }
    }
}