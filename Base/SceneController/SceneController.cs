using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

namespace DouduckGame {
	public sealed class SceneController {

		private bool m_bIsLoadingLevel = false;
		public bool isLoadingLevel {
			get {
				return m_bIsLoadingLevel;
			}
		}
		private float m_fProgress;
		private IScenePipeline m_ScenePipeline = null;

		public void SetScenePipeline(IScenePipeline oScenePipeline) {
			m_ScenePipeline = oScenePipeline;
		}

		public void UpdateScenePipeline() {
			if (m_ScenePipeline != null) {
				m_ScenePipeline.ProgressUpdate (m_fProgress);
			}
		}

		public void LoadScene(string sSceneName, bool bReload = false) {
			if (bReload) {
				LoadSceneAsync(sSceneName);
			} else if (SceneManager.GetActiveScene().name != sSceneName) {
				LoadSceneAsync(sSceneName);
			}
		}

		private void LoadSceneAsync(string sSceneName) {
			Debug.Log(string.Format("[SceneController] Start Loading Scene<{0:}>", sSceneName));
			m_bIsLoadingLevel = true;
			m_fProgress = 0f;
			if (m_ScenePipeline != null) {
				m_ScenePipeline.BeforeSceneLoading ();
			}
			Timing.RunCoroutine(StartLoadScene(sSceneName));
		}

		private IEnumerator<float> StartLoadScene(string sSceneName) {
			AsyncOperation op = SceneManager.LoadSceneAsync(sSceneName, LoadSceneMode.Single);
			while (!op.isDone) {
				m_fProgress = op.progress;
				yield return 0f;
			}
			if (m_ScenePipeline != null) {
				m_ScenePipeline.AfterSceneLoading ();
			}
			m_bIsLoadingLevel = false;
			Debug.Log(string.Format("[SceneController] Loading Scene<{0:}> is done", sSceneName));
		}
	}
}
