﻿using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib {
    public interface ISceneSaveProcessing {
        void OnSceneSave ();
    }

#if UNITY_EDITOR
    public class SceneSaveProcessor : AssetPostprocessor {
        static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            foreach (string path in importedAssets) {
                if (Path.GetExtension (path).Equals (".unity")) {
                    foreach (Transform trans in UnityUtil.GetAllTransforms ()) {
                        OnSceneSaveProcessing (trans);
                    }
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
#endif
}