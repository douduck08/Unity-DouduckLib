using UnityEngine;
using System.Collections;

namespace DouduckGame {
    public abstract class Player {
        public int PlayerNumber = 0;
        public virtual void BeforeRemove () {

        }
        public override string ToString () {
            return string.Format ("<{0}, PlayerNumber: {1}>", GetType ().Name, PlayerNumber);
        }
    }
}
