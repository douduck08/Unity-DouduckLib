using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class GameSystemMono : MonoBehaviour {
		public abstract void StartGameSystem();
		public abstract void DestoryGameSystem();
	}
}