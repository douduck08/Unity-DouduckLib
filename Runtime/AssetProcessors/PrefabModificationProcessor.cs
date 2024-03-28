using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DouduckLib
{
    public interface IPrefabSaveProcessing
    {
        // void OnPrefabApply();
        void OnPrefabSave();
    }

#if UNITY_EDITOR
    public class PrefabModificationProcessor : AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            var root = prefabStage?.prefabContentsRoot;
            if (root != null)
            {
                OnPrefabSave(root);
            }
            else
            {
                foreach (string path in paths)
                {
                    if (Path.GetExtension(path).Equals(".prefab"))
                    {
                        var go = LoadPrefab(path);
                        if (go != null)
                        {
                            OnPrefabSave(go);
                        }
                    }
                }
            }
            return paths;
        }

        static GameObject LoadPrefab(string path)
        {
            var go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            return go;
        }

        static void OnPrefabSave(GameObject go)
        {
            Component[] components = go.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (component is IPrefabSaveProcessing)
                {
                    (component as IPrefabSaveProcessing).OnPrefabSave();
                }
            }
        }
    }
#endif
}
