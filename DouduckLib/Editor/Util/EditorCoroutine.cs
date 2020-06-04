using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class EditorCoroutine {

        public static EditorCoroutine Start (IEnumerator coroutine) {
            return new EditorCoroutine (coroutine).Start ();
        }

        IEnumerator routine;

        EditorCoroutine (IEnumerator _routine) {
            routine = _routine;
        }

        EditorCoroutine Start () {
            EditorApplication.update += Update;
            return this;
        }

        public void Stop () {
            EditorApplication.update -= Update;
        }

        void Update () {
            if (!routine.MoveNext ()) {
                Stop ();
            }
        }
    }
}