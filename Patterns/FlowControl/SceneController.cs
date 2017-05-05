using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

namespace DouduckGame {
	public sealed class SceneController {

        public static event Action<string> OnSceneLoaded;
        public static event Action<string> OnSceneUnloaded;
        public static event Action<string> BeforeSceneLoading;
        public static event Action<string> AfterSceneLoading;
        public static event Action<string, float> SceneLoadingProgress;

        public static string ActiveSceneName {
            get {
                return SceneManager.GetActiveScene ().name;
            }
        }

        private bool m_bIsLoadingLevel = false;
        public bool isLoadingLevel {
            get {
                return m_bIsLoadingLevel;
            }
        }

        public SceneController () {
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            if (OnSceneLoaded != null) {
                OnSceneLoaded (scene.name);
            }
        }

        private void SceneUnloaded (Scene scene) {
            if (OnSceneUnloaded != null) {
                OnSceneUnloaded (scene.name);
            }
        }

        public void LoadScene(string sSceneName) {
            if (m_bIsLoadingLevel) {
                Util.UnityConsole.LogError ("There is a loading task processing.");
            }
            LoadSceneAsync(sSceneName);
		}

        private void LoadSceneAsync(string sSceneName) {
            Util.UnityConsole.Log(string.Format("[SceneController] Start Loading Scene<{0:}>", sSceneName));
			m_bIsLoadingLevel = true;
            if (BeforeSceneLoading != null) {
                BeforeSceneLoading (sSceneName);
            }
			Timing.RunCoroutine(StartLoadScene(sSceneName));
		}

		private IEnumerator<float> StartLoadScene(string sSceneName) {
			AsyncOperation op = SceneManager.LoadSceneAsync(sSceneName, LoadSceneMode.Single);
			while (!op.isDone) {
                if (SceneLoadingProgress != null) {
                    SceneLoadingProgress (sSceneName, op.progress);
                }
				yield return 0f;
			}
            if (AfterSceneLoading != null) {
                AfterSceneLoading (sSceneName);
            }
            m_bIsLoadingLevel = false;
            Util.UnityConsole.Log(string.Format("[SceneController] Loading Scene<{0:}> is done", sSceneName));
		}
	}
}
