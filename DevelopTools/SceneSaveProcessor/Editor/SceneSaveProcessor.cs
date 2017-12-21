using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaveProcessor : UnityEditor.AssetModificationProcessor {
    static string[] OnWillSaveAssets (string[] paths) {
        foreach (string path in paths) {
            if (Path.GetExtension (path).Equals (".unity")) {
                foreach (GameObject obj in SceneManager.GetActiveScene ().GetRootGameObjects ()) {
                    RunTaskOnTransform (obj.transform, OnSceneSaveProcessing);
                }
                return paths;
            }
        }
        return paths;
    }

    private static void RunTaskOnTransform (Transform parent, Action<Transform> task) {
        task.Invoke (parent);
        foreach (Transform trans in parent.GetComponentsInChildren<Transform> (true)) {
            if (trans != parent.transform) {
                RunTaskOnTransform (trans, task);
            }
        }
    }

    private static void OnSceneSaveProcessing (Transform trans) {
        Component[] components = trans.GetComponents<Component> ();
        foreach (Component component in components) {
            if (component is ISceneSaveProcessing) {
                (component as ISceneSaveProcessing).OnSceneSave ();
            }
        }
    }
}
