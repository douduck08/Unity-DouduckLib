using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace DouduckGame {
    [AddComponentMenu("DouduckGame/DouduckGameCore")]
	public sealed class DouduckGameCore : MonoBehaviour {
		
		[Header("You can add \"DEBUG_SWITCH\" into define symbol.")]
		[Tooltip("To use DebugMode switch, you need to add \"DEBUG_SWITCH\" into define symbol.")]
		public bool DebugMode = false;
		public bool ScreenNeverSleep = false;
		public int TargetFramerate = 60;

		public UnityEvent AppStart;
		public UnityEvent AppUpate;

		public static GameObject InstanceGameObject;

		private static GameSystemMonoManager m_SystemManager;
		private static bool m_bIsInitialized = false;

		private void Awake () {
			if (m_bIsInitialized) {
				Debug.LogError("[DouduckGameCore] was initialized");
				Object.Destroy(this);
			} else {
				m_bIsInitialized = true;

				#if DEBUG_SWITCH
				Debug.DebugMode = this.DebugMode;
				#endif

				Random.seed = System.DateTime.Now.Millisecond;
				if (ScreenNeverSleep) {
					Screen.sleepTimeout = SleepTimeout.NeverSleep;
				}
				Application.targetFrameRate = TargetFramerate;

				// *** GameCore Object Setting ***
				Debug.Log("========== App Awake ==========");
				transform.name = "[DouduckGameCore]";
				GameObject.DontDestroyOnLoad(this.gameObject);

				InstanceGameObject = this.gameObject;
				m_SystemManager = new GameSystemMonoManager (InstanceGameObject);

				// *** Show System Infomation ***
				Utility.SystemInfomation.ShowInfomation();
			}
		}

		void Start () {
			Debug.Log("========== App Start ==========");
			m_SystemManager.StartInitialSystem();
			AppStart.Invoke();
		}

		void Update () {
			AppUpate.Invoke();
		}

		// *** System manager method ***
		public static T AddSystem<T> () where T : IGameSystemMono {
			return m_SystemManager.AddSystem<T>();
		}

		public static void RemoveSystem<T> () where T : IGameSystemMono {
			m_SystemManager.RemoveSystem<T>();
		}

		public static void EnableSystem<T> () where T : IGameSystemMono {
			m_SystemManager.EnableSystem<T>();
		}

		public static void DisableSystem<T> () where T : IGameSystemMono {
			m_SystemManager.DisableSystem<T>();
		}

		public static T GetSystem<T> () where T : IGameSystemMono {
			return m_SystemManager.GetSystem<T>();
		}
	}
}
