using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib
{
    public interface IPrefabApplyProcessing
    {
        void OnPrefabApply();
    }

#if UNITY_EDITOR
    public class PrefabApplyProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (Path.GetExtension(path).Equals(".prefab"))
                {
                    var go = LoadPrefab(path);
                    if (go != null)
                    {
                        OnPrefabApply(go);
                    }
                }
            }
        }

        static GameObject LoadPrefab(string path)
        {
            var go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            return go;
        }

        static void OnPrefabApply(GameObject go)
        {
            Component[] components = go.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (component is IPrefabApplyProcessing)
                {
                    (component as IPrefabApplyProcessing).OnPrefabApply();
                }
            }
        }
    }
#endif
}
