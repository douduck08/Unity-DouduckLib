using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public class CoroutineCarrier : SingletonMonoAuto<CoroutineCarrier> {
        protected internal override void OnSingletonAwake () {
            MarkAsCrossSceneSingleton ();
        }

        public static void StartCoroutineOnDontDestroy (IEnumerator routine) {
            instance.StartCoroutine (routine);
        }
    }
}