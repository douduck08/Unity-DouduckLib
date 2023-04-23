using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor
{
    public class EditorShortCut
    {
        // Ctrl + L
        [MenuItem("Edit/Lock %l")]
        static void LockInspector()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        [MenuItem("Edit/Lock %l", true)]
        static bool LockInspectorValid()
        {
            return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
        }

        // Ctrl + Shift + U
        [MenuItem("Edit/Unselect All %#u", false, 20)]
        static void Unselect()
        {
            Selection.activeGameObject = null;
        }

        // Ctrl + G
        [MenuItem("GameObject/Group Selected GameObjects %g")]
        static void GroupSelected()
        {
            if (Selection.activeTransform == null)
            {
                return;
            }

            var go = new GameObject() { name = "Group" };
            go.transform.SetParent(Selection.activeTransform.parent, true);
            Undo.RegisterCreatedObjectUndo(go, "Group object");

            foreach (var transform in Selection.transforms)
            {
                Undo.SetTransformParent(transform, go.transform, "Group Selected");
            }
            Selection.activeObject = go;
        }
    }
}
