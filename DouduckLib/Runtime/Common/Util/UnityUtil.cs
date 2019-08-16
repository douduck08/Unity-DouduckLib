using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DouduckLib {
    public static class UnityUtil {

        public static Scene ActiveScene {
            get {
                return SceneManager.GetActiveScene ();
            }
        }

        public static IEnumerable<Scene> GetAllLoadedScenes () {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                yield return SceneManager.GetSceneAt (i);
            }
        }

        /// <summary>
        /// This method include inactive GameObject, exclude DontDestory GameObject
        /// </summary>
        public static IEnumerable<GameObject> GetRootGameObjects (Scene scene) {
            return scene.GetRootGameObjects ();
        }

        /// <summary>
        /// This method include inactive GameObject, exclude DontDestory GameObject
        /// </summary>
        public static IEnumerable<GameObject> GetRootGameObjects () {
            foreach (Scene scene in GetAllLoadedScenes ()) {
                foreach (GameObject go in GetRootGameObjects (scene)) {
                    yield return go;
                }
            }
        }

        /// <summary>
        /// This method include inactive GameObject, exclude DontDestory GameObject
        /// </summary>
        public static IEnumerable<Transform> GetAllTransforms () {
            var rootObjects = GetRootGameObjects ();
            foreach (Transform trans in rootObjects.Select (x => x.transform)) {
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

        /// <summary>
        /// This method include inactive GameObject, exclude DontDestory GameObject
        /// </summary>
        public static IEnumerable<T> GetAllComponents<T> () where T : Behaviour {
            foreach (Transform trans in GetAllTransforms ()) {
                foreach (T t in trans.GetComponents<T> ()) {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// This method include DontDestory GameObject, exclude inactive GameObject
        /// </summary>
        public static IEnumerable<GameObject> GetAllActiveGameObjects () {
            return GameObject.FindObjectsOfType<Transform> ().Select (x => x.gameObject);
        }
    }
}