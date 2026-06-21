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
            public int AddSceneByPath(string path)
            {
                var buildIndex = SceneUtility.GetBuildIndexByScenePath(path);
                if (buildIndex > -1)
                {
                    Add(buildIndex);
                    return buildIndex;
                }
                Debug.LogError($"[SceneLoader] Failed to add scene by path: '{path}'. It may not be added to the Build Settings.");
                return -1;
            }

            public int AddSceneByIndex(int index)
            {
                var sceneCount = SceneManager.sceneCountInBuildSettings;
                if (index >= 0 && index < sceneCount)
                {
                    Add(index);
                    return index;
                }
                Debug.LogError($"[SceneLoader] Failed to add scene by index: {index}. Valid index range is 0 to {sceneCount - 1}.");
                return -1;
            }
        }

        public event Action OnStartLoading;
        public event Action OnFinishLoading;
        public event Action<Scene> OnSceneUnloaded;
        public event Action<Scene, LoadSceneMode> OnSceneLoaded;
        public event Action<float> OnUpdateProgress;

        public bool IsLoadingScene { get; protected set; }
        public bool IsLoadingOver { get; protected set; }

        int _activeSceneIndex = -1;
        bool _useManualActivation;
        bool _triggerActivation;
        Action _onReadyToActivate;

        SceneIndexList _loadList = new();
        SceneIndexList _unloadList = new();

        public static SceneLoader Create()
        {
            return new SceneLoader();
        }

        SceneLoader()
        {
        }

        public SceneLoader AddSceneToLoadByPath(string path, bool setActive = false)
        {
            var buildIndex = _loadList.AddSceneByPath(path);
            if (setActive && buildIndex > -1)
            {
                _activeSceneIndex = buildIndex;
            }
            return this;
        }

        public SceneLoader AddSceneToLoadByIndex(int index, bool setActive = false)
        {
            var buildIndex = _loadList.AddSceneByIndex(index);
            if (setActive && buildIndex > -1)
            {
                _activeSceneIndex = buildIndex;
            }
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

        public SceneLoader SetManualActivation(Action onReadyToActivate)
        {
            _useManualActivation = true;
            _onReadyToActivate = onReadyToActivate;
            return this;
        }


        public void ActivateLoadedScenes()
        {
            _triggerActivation = true;
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

            try
            {
                var unloadOps = new List<AsyncOperation>();
                for (int i = 0; i < _unloadList.Count; i++)
                {
                    if (_unloadList[i] > -1)
                    {
                        var scene = SceneManager.GetSceneByBuildIndex(_unloadList[i]);
                        if (scene.isLoaded)
                        {
                            var op = SceneManager.UnloadSceneAsync(_unloadList[i]);
                            if (op != null)
                            {
                                unloadOps.Add(op);
                            }
                        }
                    }
                }

                var loadOps = new List<AsyncOperation>();
                for (int i = 0; i < _loadList.Count; i++)
                {
                    if (_loadList[i] > -1)
                    {
                        var op = SceneManager.LoadSceneAsync(_loadList[i], LoadSceneMode.Additive);
                        if (op != null)
                        {
                            op.allowSceneActivation = !_useManualActivation;
                            loadOps.Add(op);
                        }
                    }
                }

                var activeLoadOps = new List<AsyncOperation>(loadOps);
                var loadOpsCount = activeLoadOps.Count;
                bool hasTriggeredReady = false;

                while (unloadOps.Count > 0 || activeLoadOps.Count > 0)
                {
                    yield return null;

                    for (int i = unloadOps.Count - 1; i >= 0; i--)
                    {
                        if (unloadOps[i].isDone)
                        {
                            unloadOps.RemoveAt(i);
                        }
                    }

                    bool allReadyForActivation = true;
                    var totalProgress = 0f;
                    for (int i = activeLoadOps.Count - 1; i >= 0; i--)
                    {
                        var op = activeLoadOps[i];
                        if (op.isDone)
                        {
                            activeLoadOps.RemoveAt(i);
                        }
                        else
                        {
                            if (op.progress < 0.9f)
                            {
                                allReadyForActivation = false;
                            }
                            totalProgress += Mathf.Clamp01(op.progress / 0.9f);
                        }
                    }

                    if (loadOpsCount > 0)
                    {
                        totalProgress = (totalProgress + (loadOpsCount - activeLoadOps.Count)) / loadOpsCount;
                        OnUpdateProgress?.Invoke(totalProgress);
                    }
                    else
                    {
                        OnUpdateProgress?.Invoke(1.0f);
                    }

                    if (_useManualActivation && !hasTriggeredReady && allReadyForActivation && unloadOps.Count == 0)
                    {
                        hasTriggeredReady = true;
                        _onReadyToActivate?.Invoke();

                        yield return new WaitUntil(() => _triggerActivation);

                        for (int i = 0; i < activeLoadOps.Count; i++)
                        {
                            activeLoadOps[i].allowSceneActivation = true;
                        }
                    }
                }

                if (_activeSceneIndex > -1)
                {
                    var activeScene = SceneManager.GetSceneByBuildIndex(_activeSceneIndex);
                    if (activeScene.IsValid())
                    {
                        SceneManager.SetActiveScene(activeScene);
                    }
                }
            }
            finally
            {
                IsLoadingScene = false;
                IsLoadingOver = true;
                OnFinishLoading?.Invoke();

                SceneManager.sceneLoaded -= TriggerSceneLoaded;
                SceneManager.sceneUnloaded -= TriggerSceneUnloaded;
            }
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
