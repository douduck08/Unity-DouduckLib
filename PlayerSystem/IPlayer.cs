using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public abstract class IPlayer {
		public int PlayerNumber;
		public uint UID;
		public string Name;

		public IPlayer () {
			PlayerNumber = 0;
			UID = 0;
			Name = null;
		}

		public virtual void BeforeRemove() {
			
		}
	}
}
