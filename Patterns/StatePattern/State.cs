using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public abstract class State : StateBase {
        protected void TransTo (State state) {
            Controller.TransTo (state, StateLevel);
        }
    }
}
