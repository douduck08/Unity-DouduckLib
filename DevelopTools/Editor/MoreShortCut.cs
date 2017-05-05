using UnityEngine;
using UnityEditor;

public class MoreShortCut {

    [MenuItem ("Edit/Lock %l")]
    public static void LockInspector () {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild ();
    }

    [MenuItem ("Edit/Lock %l", true)]
    public static bool LockInspectorValid () {
        return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
    }
}
