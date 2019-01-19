using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public abstract class EditorWindowBase<T> : EditorWindow where T : EditorWindow {

        protected static void Open (string title) {
            T window = EditorWindow.GetWindow<T> (false, title, true);
            window.minSize = new Vector2 (256, 256);
        }

        protected Object target;
        protected SerializedObject serializedObject;

        Vector2 scrollPos;

        void Initialize () {
            if (target == null) {
                target = this;
                serializedObject = new SerializedObject (target);
            }
        }

        protected void OnFocus () {
            Initialize ();
        }

        protected void OnLostFocus () { }

        protected abstract void OnDrawGUIBody ();

        protected void OnGUI () {
            using (new EditorGUILayout.HorizontalScope ()) {
                EditorGUIWrapper.DrawHeader (titleContent.text);

                // TODO: menu item
                // var gearStyle = new GUIStyle ("Icon.Options");
                // if (GUILayout.Button (gearStyle.normal.background, new GUIStyle ("IconButton"), GUILayout.MaxWidth (20))) {
                //     var menu = new GenericMenu ();
                //     menu.AddItem (new GUIContent ("Close"), false, data => {
                //         var item = data as EditorWindow;
                //         item.Close ();
                //     }, this);
                //     menu.ShowAsContext ();
                // }
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope (scrollPos)) {
                scrollPos = scrollView.scrollPosition;
                OnDrawGUIBody ();
            }
        }
    }
}