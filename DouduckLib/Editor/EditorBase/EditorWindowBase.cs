using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public abstract class EditorWindowBase<T> : EditorWindow where T : EditorWindow {

        protected static void Open (string title) {
            T window = EditorWindow.GetWindow<T> (false, title, true);
        }

        protected Object target;
        protected SerializedObject serializedObject;

        void Initialize () {
            if (target == null) {
                target = this;
                serializedObject = new SerializedObject (target);
            }
        }

        protected void OnFocus () {
            Initialize ();
        }

    }
}