using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public SceneLoader (int sceneToLoad, bool additive = false) {
            this.sceneIndexToLoad = sceneToLoad;
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public SceneLoader (int sceneToLoad, int sceneToUnload, bool additive = false) {
            this.sceneIndexToLoad = sceneToLoad;
            this.sceneIndexToUnload = sceneToUnload;
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public SceneLoader (string sceneToLoad, bool additive = false) {
            this.sceneToLoad = sceneToLoad;
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public SceneLoader (string sceneToLoad, string sceneToUnload, bool additive = false) {
            this.sceneToLoad = sceneToLoad;
            this.sceneToUnload = sceneToUnload;
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public void StartLoading (MonoBehaviour owner) {
            if (isLoadingScene || isLoadingOver) {
                return;
            }
            if (owner == null) {
                // TODO: generate a gameobject if owner is null
                return;
            }
            owner.StartCoroutine (LoadingScene ());
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
                unloadop = SceneManager.UnloadSceneAsync (sceneToUnload);
                while (!unloadop.isDone) {
                    yield return null;
                }
            } else if (sceneIndexToUnload > -1) {
                unloadop = SceneManager.UnloadSceneAsync (sceneIndexToUnload);
                while (!unloadop.isDone) {
                    yield return null;
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