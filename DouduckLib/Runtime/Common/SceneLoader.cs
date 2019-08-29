using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// === Start Coroutine to Load SceneB ===
// # SceneA: OnDisable
// # SceneA: OnDistroy
// SceneManager.SceneUnoaded triggered
// # SceneB: Awake
// # SceneB: OnEnable
// SceneManager.SceneLoaded triggered
// SceneManager.ActiveSceneChanged triggered
// # SceneB: Start
// # SceneB: Update
// === Coroutine End ===
// # SceneB: LateUpdate

namespace DouduckLib {
    public class SceneLoader {
        public event Action onStartLoading;
        public event Action onFinishLoading;
        public event Action<Scene> onSceneUnloaded;
        public event Action<Scene, LoadSceneMode> onSceneLoaded;
        public event Action<float> onUpdateProgress;

        public LoadSceneMode loadSceneMode { get; protected set; }
        public bool isLoadingScene { get; protected set; }
        public bool isLoadingOver { get; protected set; }

        int sceneIndexToLoad = -1;
        int sceneIndexToUnload = -1;
        string sceneToLoad;
        string sceneToUnload;
        bool setActive;

        public SceneLoader (int sceneToLoad, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.sceneIndexToLoad = sceneToLoad;
            this.setActive = setActive;
        }

        public SceneLoader (int sceneToLoad, int sceneToUnload, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.sceneIndexToLoad = sceneToLoad;
            this.sceneIndexToUnload = sceneToUnload;
        }

        public SceneLoader (string sceneToLoad, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.sceneToLoad = sceneToLoad;
            this.setActive = setActive;
            this.setActive = setActive;
        }

        public SceneLoader (string sceneToLoad, string sceneToUnload, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.sceneToLoad = sceneToLoad;
            this.sceneToUnload = sceneToUnload;
            this.setActive = setActive;
        }

        public void StartLoading (MonoBehaviour owner = null) {
            if (isLoadingScene || isLoadingOver) {
                return;
            }
            if (owner == null) {
                CoroutineCarrier.StartCoroutineOnDontDestroy (LoadingScene ());
            } else {
                owner.StartCoroutine (LoadingScene ());
            }
        }

        void DestroyDependGameObject (GameObject go) {
            GameObject.Destroy (go);
        }

        IEnumerator LoadingScene () {
            isLoadingScene = true;
            if (onStartLoading != null) {
                onStartLoading.Invoke ();
            }

            SceneManager.sceneLoaded += TriggerSceneLoaded;
            SceneManager.sceneUnloaded += TriggerSceneUnloaded;

            AsyncOperation unloadop = null;
            if (!string.IsNullOrEmpty (sceneToUnload)) {
                var scene = SceneManager.GetSceneByPath (sceneToUnload);
                if (scene.isLoaded) {
                    unloadop = SceneManager.UnloadSceneAsync (sceneToUnload);
                    while (!unloadop.isDone) {
                        yield return null;
                    }
                }
            } else if (sceneIndexToUnload > -1) {
                var scene = SceneManager.GetSceneByBuildIndex (sceneIndexToUnload);
                if (scene.isLoaded) {
                    unloadop = SceneManager.UnloadSceneAsync (sceneIndexToUnload);
                    while (!unloadop.isDone) {
                        yield return null;
                    }
                }
            }

            AsyncOperation loadop = null;
            if (!string.IsNullOrEmpty (sceneToLoad)) {
                loadop = SceneManager.LoadSceneAsync (sceneToLoad, loadSceneMode);
                while (!loadop.isDone) {
                    if (onUpdateProgress != null) {
                        onUpdateProgress.Invoke (loadop.progress);
                    }
                    yield return null;
                }
            } else if (sceneIndexToLoad > -1) {
                loadop = SceneManager.LoadSceneAsync (sceneIndexToLoad, loadSceneMode);
                while (!loadop.isDone) {
                    if (onUpdateProgress != null) {
                        onUpdateProgress.Invoke (loadop.progress);
                    }
                    yield return null;
                }
            }

            if (setActive && !string.IsNullOrEmpty (sceneToLoad)) {
                yield return new WaitForEndOfFrame ();
                var activeScene = SceneManager.GetSceneByPath (sceneToLoad);
                SceneManager.SetActiveScene (activeScene);
            } else if (setActive && sceneIndexToLoad > -1) {
                yield return null;
                var activeScene = SceneManager.GetSceneByBuildIndex (sceneIndexToLoad);
                SceneManager.SetActiveScene (activeScene);
            }

            isLoadingScene = false;
            isLoadingOver = true;
            if (onFinishLoading != null) {
                onFinishLoading.Invoke ();
            }

            SceneManager.sceneLoaded -= TriggerSceneLoaded;
            SceneManager.sceneUnloaded -= TriggerSceneUnloaded;
        }

        public SceneLoader OnSceneUnloaded (Action<Scene> callback) {
            onSceneUnloaded += callback;
            return this;
        }

        public SceneLoader OnSceneLoaded (Action<Scene, LoadSceneMode> callback) {
            onSceneLoaded += callback;
            return this;
        }

        public SceneLoader OnStartLoading (Action callback) {
            onStartLoading += callback;
            return this;
        }

        public SceneLoader OnFinishLoading (Action callback) {
            onFinishLoading += callback;
            return this;
        }

        public SceneLoader OnUpdateProgress (Action<float> callback) {
            onUpdateProgress += callback;
            return this;
        }

        void TriggerSceneUnloaded (Scene scene) {
            if (onSceneUnloaded != null) {
                onSceneUnloaded.Invoke (scene);
            }
        }

        void TriggerSceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {
            if (onSceneLoaded != null) {
                onSceneLoaded.Invoke (scene, loadSceneMode);
            }
        }
    }
}