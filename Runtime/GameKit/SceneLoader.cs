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

namespace DouduckLib
{
    public class SceneLoader
    {

        class SceneIndexList : List<int>
        {
            public bool AddSceneByPath(string path)
            {
                var buildIndex = SceneUtility.GetBuildIndexByScenePath(path);
                if (buildIndex > -1)
                {
                    Add(buildIndex);
                    return true;
                }
                return false;
            }

            public bool AddSceneByIndex(int index)
            {
                if (index > -1)
                {
                    Add(index);
                    return true;
                }
                return false;
            }
        }

        public event Action OnStartLoading;
        public event Action OnFinishLoading;
        public event Action<Scene> OnSceneUnloaded;
        public event Action<Scene, LoadSceneMode> OnSceneLoaded;
        public event Action<float> OnUpdateProgress;

        public LoadSceneMode LoadSceneMode { get; protected set; }
        public bool IsLoadingScene { get; protected set; }
        public bool IsLoadingOver { get; protected set; }

        // int sceneIndexToLoad = -1;
        // int sceneIndexToUnload = -1;
        // string sceneToLoad;
        // string sceneToUnload;

        bool _setActive;
        SceneIndexList _loadList = new();
        SceneIndexList _unloadList = new();

        public static SceneLoader Create(bool additive = true, bool setActiveAfterLoaded = false)
        {
            return new SceneLoader(additive, setActiveAfterLoaded);
        }

        SceneLoader(bool additive, bool setActiveAfterLoaded)
        {
            LoadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            _setActive = setActiveAfterLoaded;
        }

        public SceneLoader AddSceneToLoadByPath(string path)
        {
            _loadList.AddSceneByPath(path);
            return this;
        }

        public SceneLoader AddSceneToLoadByIndex(int index)
        {
            _loadList.AddSceneByIndex(index);
            return this;
        }

        public SceneLoader AddSceneToLoadByPath(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                _loadList.AddSceneByPath(path);
            }
            return this;
        }

        public SceneLoader AddSceneToLoadByIndex(IEnumerable<int> indexes)
        {
            foreach (var index in indexes)
            {
                _loadList.AddSceneByIndex(index);
            }
            return this;
        }

        public SceneLoader AddSceneToUnloadByPath(string path)
        {
            _unloadList.AddSceneByPath(path);
            return this;
        }

        public SceneLoader AddSceneToUnloadByIndex(int index)
        {
            _unloadList.AddSceneByIndex(index);
            return this;
        }

        public SceneLoader AddSceneToUnloadByPath(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                _unloadList.AddSceneByPath(path);
            }
            return this;
        }

        public SceneLoader AddSceneToUnloadByIndex(IEnumerable<int> indexes)
        {
            foreach (var index in indexes)
            {
                _unloadList.AddSceneByIndex(index);
            }
            return this;
        }

        public SceneLoader SetOnSceneUnloaded(Action<Scene> callback)
        {
            OnSceneUnloaded += callback;
            return this;
        }

        public SceneLoader SetOnSceneLoaded(Action<Scene, LoadSceneMode> callback)
        {
            OnSceneLoaded += callback;
            return this;
        }

        public SceneLoader SetOnStartLoading(Action callback)
        {
            OnStartLoading += callback;
            return this;
        }

        public SceneLoader SetOnFinishLoading(Action callback)
        {
            OnFinishLoading += callback;
            return this;
        }

        public SceneLoader SetOnUpdateProgress(Action<float> callback)
        {
            OnUpdateProgress += callback;
            return this;
        }

        public void StartProcessing(MonoBehaviour owner = null)
        {
            if (IsLoadingScene || IsLoadingOver)
            {
                return;
            }
            if (owner == null)
            {
                CoroutineUtil.StartCoroutineOnDontDestroy(LoadingScene());
            }
            else
            {
                owner.StartCoroutine(LoadingScene());
            }
        }

        IEnumerator LoadingScene()
        {
            IsLoadingScene = true;
            OnStartLoading?.Invoke();

            SceneManager.sceneLoaded += TriggerSceneLoaded;
            SceneManager.sceneUnloaded += TriggerSceneUnloaded;

            var unloadOps = new List<AsyncOperation>();
            for (int i = 0; i < _unloadList.Count; i++)
            {
                if (_unloadList[i] > -1)
                {
                    var scene = SceneManager.GetSceneByBuildIndex(_unloadList[i]);
                    if (scene.isLoaded)
                    {
                        unloadOps.Add(SceneManager.UnloadSceneAsync(_unloadList[i]));
                    }
                }
            }

            var loadOps = new List<AsyncOperation>();
            for (int i = 0; i < _loadList.Count; i++)
            {
                if (_loadList[i] > -1)
                {
                    loadOps.Add(SceneManager.LoadSceneAsync(_loadList[i], LoadSceneMode));
                }
            }

            var loadOpsCount = loadOps.Count;
            while (unloadOps.Count > 0 || loadOps.Count > 0)
            {
                yield return null;

                for (int i = unloadOps.Count - 1; i >= 0; i--)
                {
                    if (unloadOps[i].isDone)
                    {
                        unloadOps.RemoveAt(i);
                    }
                }

                var totalProgress = 0f;
                for (int i = loadOps.Count - 1; i >= 0; i--)
                {
                    if (loadOps[i].isDone)
                    {
                        loadOps.RemoveAt(i);
                    }
                    else
                    {
                        totalProgress += loadOps[i].progress;
                    }
                }
                totalProgress = (totalProgress + 1f * (loadOpsCount - loadOps.Count)) / loadOpsCount;
                OnUpdateProgress?.Invoke(totalProgress);
            }

            if (_setActive)
            {
                for (int i = 0; i < _loadList.Count; i++)
                {
                    if (_loadList[i] > -1)
                    {
                        var activeScene = SceneManager.GetSceneByBuildIndex(_loadList[i]);
                        SceneManager.SetActiveScene(activeScene);
                    }
                }
            }

            IsLoadingScene = false;
            IsLoadingOver = true;
            OnFinishLoading?.Invoke();

            SceneManager.sceneLoaded -= TriggerSceneLoaded;
            SceneManager.sceneUnloaded -= TriggerSceneUnloaded;
        }

        void TriggerSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            OnSceneLoaded?.Invoke(scene, loadSceneMode);
        }

        void TriggerSceneUnloaded(Scene scene)
        {
            OnSceneUnloaded?.Invoke(scene);
        }
    }
}
