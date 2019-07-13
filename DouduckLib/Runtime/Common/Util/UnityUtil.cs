using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DouduckLib {
    public static class UnityUtil {

        public static IEnumerable<Scene> AllScenes {
            get {
                for (int i = 0; i < SceneManager.sceneCount; i++) {
                    yield return SceneManager.GetSceneAt (i);
                }
            }
        }

        public static IEnumerable<Scene> AllLoadedScenes {
            get {
                return AllScenes.Where (scene => scene.isLoaded);
            }
        }

        /// <summary>
        /// This property include inactive GameObject, exclude DontDestory GameObject
        /// </summary>
        public static IEnumerable<GameObject> AllRootGameObjects {
            get {
                return SceneManager.GetActiveScene ().GetRootGameObjects ();
            }
        }

        /// <summary>
        /// This property include inactive GameObject, exclude DontDestory GameObject
        /// </summary>
        public static IEnumerable<Transform> AllTransforms {
            get {
                var rootObjects = AllRootGameObjects;
                foreach (Transform trans in AllRootGameObjects.Select (x => x.transform)) {
                    yield return trans;
                    if (trans.childCount > 0) {
                        foreach (Transform child in trans.GetComponentsInChildren<Transform> (true)) {
                            if (child != trans) {
                                yield return child;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method include inactive GameObject, exclude DontDestory GameObject
        /// </summary>
        public static IEnumerable<T> GetAllComponents<T> () where T : Behaviour {
            foreach (Transform trans in AllTransforms) {
                foreach (T t in trans.GetComponents<T> ()) {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// This property include DontDestory GameObject, exclude inactive GameObject
        /// </summary>
        public static IEnumerable<GameObject> AllActiveGameObjects {
            get {
                return GameObject.FindObjectsOfType<Transform> ().Select (x => x.gameObject);
            }
        }
    }
}