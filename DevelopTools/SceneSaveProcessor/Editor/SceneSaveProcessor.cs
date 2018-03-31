using System;
using System.IO;
using UnityEngine;
using DouduckLib;

namespace DouduckLibEditor {
    public class SceneSaveProcessor : UnityEditor.AssetModificationProcessor {
        static string[] OnWillSaveAssets (string[] paths) {
            foreach (string path in paths) {
                if (Path.GetExtension (path).Equals (".unity")) {
                    foreach (Transform trans in UnityUtil.AllTransforms) {
                        OnSceneSaveProcessing (trans);
                    }
                    return paths;
                }
            }
            return paths;
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
}