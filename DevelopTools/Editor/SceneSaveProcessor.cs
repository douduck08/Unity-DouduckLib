using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaveProcessor : UnityEditor.AssetModificationProcessor {
    static string[] OnWillSaveAssets (string[] paths) {
        foreach (GameObject obj in SceneManager.GetActiveScene ().GetRootGameObjects ()) {
            DoTaskInTransform (obj.transform);
        }
        return paths;
    }

    private static void DoTaskInTransform (Transform parent) {
        Task (parent);
        foreach (Transform trans in parent.GetComponentsInChildren<Transform> (true)) {
            if (trans != parent.transform) {
                DoTaskInTransform (trans);
            }
        }
    }

    private static void Task(Transform trans) {
        Component[] components = trans.GetComponents<Component> ();
        foreach (Component component in components) {
            if (component is IBeforeSaveScene) {
                (component as IBeforeSaveScene).BeforeSaveScene ();
            }
        }
    } 
}

