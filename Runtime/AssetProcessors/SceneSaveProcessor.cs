using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DouduckLib
{
    public interface ISceneSaveProcessing
    {
        void OnSceneSave();
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    static class SceneSaveProcessor
    {
        static SceneSaveProcessor()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
        }

        static void OnSceneSaving(Scene scene, string path)
        {
            foreach (Transform trans in UnityUtil.GetAllTransforms())
            {
                OnSceneSaveProcessing(trans);
            }
        }

        static void OnSceneSaveProcessing(Transform trans)
        {
            Component[] components = trans.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component is ISceneSaveProcessing)
                {
                    (component as ISceneSaveProcessing).OnSceneSave();
                }
            }
        }
    }
#endif
}
