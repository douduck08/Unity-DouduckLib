using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class EditorShortCut {

        [MenuItem ("Edit/Lock %l")]
        public static void LockInspector () {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild ();
        }

        [MenuItem ("Edit/Lock %l", true)]
        public static bool LockInspectorValid () {
            return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
        }

        [MenuItem ("Edit/Unselect All %#a", false, 20)]
        public static void Unselect () {
            Selection.activeGameObject = null;
        }
    }
}