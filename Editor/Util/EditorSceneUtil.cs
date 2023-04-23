using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DouduckLibEditor
{

    public static class EditorSceneUtil
    {

        public static IEnumerable<string> GetAllScenePaths()
        {
            var guids = AssetDatabase.FindAssets("t:Scene");
            var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).Where(p => File.Exists(p));
            return paths;
        }

        public static IEnumerable<string> GetAllScenesInBuild()
        {
            for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                yield return SceneUtility.GetScenePathByBuildIndex(i);
            }
        }

        public static void ProcessAllScenes(Action<Scene> progress)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            var paths = GetAllScenePaths().ToArray();
            EditorCoroutine.Start(ProcessScenesCoroutine(paths, progress));
        }

        public static void ProcessAllScenesInBuild(Action<Scene> progress)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            var paths = GetAllScenesInBuild().ToArray();
            EditorCoroutine.Start(ProcessScenesCoroutine(paths, progress));
        }

        static IEnumerator ProcessScenesCoroutine(string[] scenePaths, Action<Scene> progress)
        {
            var sceneCount = scenePaths.Length;
            for (int i = 0; i < sceneCount; i++)
            {
                var scenePath = scenePaths[i];

                Debug.LogFormat("Processing scene {0}/{1}: {2}", i + 1, sceneCount, scenePath);

                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                yield return null;

                if (progress != null)
                {
                    progress.Invoke(scene);
                }
            }

            // EditorUtility.ClearProgressBar ();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        }
    }
}
