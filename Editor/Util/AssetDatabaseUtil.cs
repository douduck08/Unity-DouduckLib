using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor
{
    public static class AssetDatabaseUtil
    {
        public static T FindOrCreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            if (!path.StartsWith("Assets/"))
            {
                throw new System.InvalidOperationException("'path' should start with \"Assets/\"");
            }
            if (Path.GetFileNameWithoutExtension(path).Equals(""))
            {
                throw new System.InvalidOperationException("'path' should contain the file name");
            }

            var folderPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (!Path.GetExtension(path).Equals(".asset"))
            {
                path += ".asset";
            }

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                // Debug.Log("Create asset: " + path);
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }
            return asset;
        }

        public static T FindOrCreateScriptableObject<T>(string folderPath, string fileName) where T : ScriptableObject
        {
            var path = Path.Combine(folderPath, fileName);
            return FindOrCreateScriptableObject<T>(path);
        }
    }
}
