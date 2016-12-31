using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class IScenePipeline {
		public virtual void ProgressUpdate(float fProgress) {}
		public virtual void BeforeSceneLoading() {}
		public virtual void AfterSceneLoading() {}
	}
}
