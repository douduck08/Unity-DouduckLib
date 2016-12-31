using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class IGameSystemMono : MonoBehaviour {
		public abstract void StartGameSystem();
		public abstract void DestoryGameSystem();
	}
}