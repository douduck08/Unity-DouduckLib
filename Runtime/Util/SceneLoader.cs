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

        class SceneIndexList : List<int> {
            public bool AddSceneByPath (string path) {
                var buildIndex = SceneUtility.GetBuildIndexByScenePath (path);
                if (buildIndex > -1) {
                    Add (buildIndex);
                    return true;
                }
                return false;
            }

            public bool AddSceneByIndex (int index) {
                if (index > -1) {
                    Add (index);
                    return true;
                }
                return false;
            }
        }

        public event Action onStartLoading;
        public event Action onFinishLoading;
        public event Action<Scene> onSceneUnloaded;
        public event Action<Scene, LoadSceneMode> onSceneLoaded;
        public event Action<float> onUpdateProgress;

        public LoadSceneMode loadSceneMode { get; protected set; }
        public bool isLoadingScene { get; protected set; }
        public bool isLoadingOver { get; protected set; }

        // int sceneIndexToLoad = -1;
        // int sceneIndexToUnload = -1;
        // string sceneToLoad;
        // string sceneToUnload;

        bool setActive;
        SceneIndexList loadList = new SceneIndexList ();
        SceneIndexList unloadList = new SceneIndexList ();

        public SceneLoader (bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.setActive = setActive;
        }

        public SceneLoader (int loadSceneIndex, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.setActive = setActive;
            loadList.AddSceneByIndex (loadSceneIndex);
        }

        public SceneLoader (int loadSceneIndex, int unloadSceneIndex, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            loadList.AddSceneByIndex (loadSceneIndex);
            unloadList.AddSceneByIndex (unloadSceneIndex);
        }

        public SceneLoader (string loadScenePath, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.setActive = setActive;
            loadList.AddSceneByPath (loadScenePath);
        }

        public SceneLoader (string loadScenePath, string unloadScenePath, bool additive = false, bool setActive = false) {
            loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.setActive = setActive;
            loadList.AddSceneByPath (loadScenePath);
            unloadList.AddSceneByPath (unloadScenePath);
        }

        public SceneLoader AddLoadSceneByPath (string path) {
            loadList.AddSceneByPath (path);
            return this;
        }

        public SceneLoader AddLoadSceneByIndex (int index) {
            loadList.AddSceneByIndex (index);
            return this;
        }

        public SceneLoader AddLoadSceneByPath (IEnumerable<string> paths) {
            foreach (var path in paths) {
                loadList.AddSceneByPath (path);
            }
            return this;
        }

        public SceneLoader AddLoadSceneByIndex (IEnumerable<int> indexes) {
            foreach (var index in indexes) {
                loadList.AddSceneByIndex (index);
            }
            return this;
        }

        public SceneLoader AddUnloadSceneByPath (string path) {
            unloadList.AddSceneByPath (path);
            return this;
        }

        public SceneLoader AddUnloadSceneByIndex (int index) {
            unloadList.AddSceneByIndex (index);
            return this;
        }

        public SceneLoader AddUnloadSceneByPath (IEnumerable<string> paths) {
            foreach (var path in paths) {
                unloadList.AddSceneByPath (path);
            }
            return this;
        }

        public SceneLoader AddUnloadSceneByIndex (IEnumerable<int> indexes) {
            foreach (var index in indexes) {
                unloadList.AddSceneByIndex (index);
            }
            return this;
        }

        public void StartProcessing (MonoBehaviour owner = null) {
            if (isLoadingScene || isLoadingOver) {
                return;
            }
            if (owner == null) {
                CoroutineUtil.StartCoroutineOnDontDestroy (LoadingScene ());
            } else {
                owner.StartCoroutine (LoadingScene ());
            }
        }

        IEnumerator LoadingScene () {
            isLoadingScene = true;
            if (onStartLoading != null) {
                onStartLoading.Invoke ();
            }

            SceneManager.sceneLoaded += TriggerSceneLoaded;
            SceneManager.sceneUnloaded += TriggerSceneUnloaded;

            var unloadOps = new List<AsyncOperation> ();
            for (int i = 0; i < unloadList.Count; i++) {
                if (unloadList[i] > -1) {
                    unloadOps.Add (SceneManager.UnloadSceneAsync (unloadList[i]));
                }
            }

            while (unloadOps.Count > 0) {
                yield return null;
                for (int i = unloadOps.Count - 1; i >= 0; i--) {
                    if (unloadOps[i].isDone) {
                        unloadOps.RemoveAt (i);
                    }
                }
            }

            var loadOps = new List<AsyncOperation> ();
            for (int i = 0; i < loadList.Count; i++) {
                if (loadList[i] > -1) {
                    loadOps.Add (SceneManager.LoadSceneAsync (loadList[i], loadSceneMode));
                }
            }

            var loadOpsCount = loadOps.Count;
            while (loadOps.Count > 0) {
                yield return null;

                var totalProgress = 0f;
                for (int i = loadOps.Count - 1; i >= 0; i--) {
                    if (loadOps[i].isDone) {
                        loadOps.RemoveAt (i);
                    } else {
                        totalProgress += loadOps[i].progress;
                    }
                }

                totalProgress = (totalProgress + 1f * (loadOpsCount - loadOps.Count)) / loadOpsCount;
                if (onUpdateProgress != null) {
                    onUpdateProgress.Invoke (totalProgress);
                }
            }

            if (setActive) {
                for (int i = 0; i < loadList.Count; i++) {
                    if (loadList[i] > -1) {
                        var activeScene = SceneManager.GetSceneByBuildIndex (loadList[i]);
                        SceneManager.SetActiveScene (activeScene);
                    }
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