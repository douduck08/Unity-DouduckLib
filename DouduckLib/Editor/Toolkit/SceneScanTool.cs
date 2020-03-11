using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace DouduckLibEditor {
    public static class SceneScanTool {

        public static IEnumerable<Scene> GetAllScenes () {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                yield return EditorSceneManager.GetSceneAt (i);
            }
        }

        public static void ScanScenes (IEnumerable<string> scenes, Action<Scene> progress) {
            // if (!EditorSceneManager.EnsureUntitledSceneHasBeenSaved ("Please save all scenes before Scanning")) {
            //     return;
            // }

            foreach (var scenePath in scenes) {
                var scene = EditorSceneManager.OpenScene (scenePath, OpenSceneMode.Additive);
                if (progress != null) {
                    progress.Invoke (scene);
                }
                EditorSceneManager.CloseScene (scene, true);
            }
        }

        public static void ScanAllScenes (Action<Scene> progress) {
            ScanScenes (GetAllScenes ().Select (s => s.name), progress);
        }
    }
}