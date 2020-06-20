using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class EditorShortCut {

        [MenuItem ("Edit/Lock %l")]
        static void LockInspector () {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild ();
        }

        [MenuItem ("Edit/Lock %l", true)]
        static bool LockInspectorValid () {
            return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
        }

        [MenuItem ("Edit/Unselect All %#a", false, 20)]
        static void Unselect () {
            Selection.activeGameObject = null;
        }

        [MenuItem ("GameObject/Group Selected %g")]
        static void GroupSelected () {
            if (Selection.activeTransform == null) {
                return;
            }

            var go = new GameObject () { name = "Group" };
            go.transform.SetParent (Selection.activeTransform.parent, false);
            Undo.RegisterCreatedObjectUndo (go, "Group object");

            foreach (var transform in Selection.transforms) {
                Undo.SetTransformParent (transform, go.transform, "Group Selected");
            }
            Selection.activeObject = go;
        }
    }
}