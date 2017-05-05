using System.Collections;
using UnityEngine;

namespace DouduckGame {
    public interface StateControllerBase {
        void Terminate ();
        void TransTo (StateBase oState, int iLevel);
        void StateUpdate ();
    }
}
