using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public static class CoroutineHandle {
        public static IEnumerator DelayEnumerator (float seconds, IEnumerator enumerator, Action callback) {
            var wait = new WaitForSeconds (seconds);

            yield return wait;
            yield return enumerator;

            if (callback != null) {
            callback.Invoke ();
            }
        }

        public static IEnumerator CallbackEnumerator (IEnumerator enumerator, Action callback) {
            yield return enumerator;

            if (callback != null) {
            callback.Invoke ();
            }
        }

        public static IEnumerator ParallelEnumerator (MonoBehaviour owner, Action callback, params IEnumerator[] enumerators) {
            var counter = enumerators.Length;
            var wait = new WaitUntil (() => counter == 0);

            for (int i = 0; i < enumerators.Length; i++) {
            owner.StartCoroutine (CallbackEnumerator (enumerators[i], () => counter--));
            }
            yield return wait;

            if (callback != null) {
                callback.Invoke ();
            }
        }

        public static IEnumerator QueueEnumerator (Action callback, params IEnumerator[] enumerators) {
            for (int i = 0; i < enumerators.Length; i++) {
            yield return enumerators[i];
            }

            if (callback != null) {
                callback.Invoke ();
            }
        }
    }
}