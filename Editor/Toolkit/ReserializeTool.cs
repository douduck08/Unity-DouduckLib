using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace DouduckLibEditor
{
    public static class ReserializeTool
    {
        [MenuItem("Assets/Reserialize")]
        static void ReserializeSelectedAssets()
        {
            var assetPaths = new List<string>();
            foreach (var guid in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                assetPaths.Add(path);
            }
            if (assetPaths.Count > 0)
            {
                AssetDatabase.ForceReserializeAssets(assetPaths);
            }
        }

        [MenuItem("Assets/Reserialize", true)]
        static bool ReserializeSelectedAssetsValidate()
        {
            return Selection.assetGUIDs.Length > 0;
        }

        // [MenuItem("Assets/Reserialize All Assets")]
        // static void ReserializeAllAssets()
        // {
        //     AssetDatabase.ForceReserializeAssets();
        // }
    }
}
