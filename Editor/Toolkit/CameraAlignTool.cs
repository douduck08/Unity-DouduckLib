using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor
{
    [InitializeOnLoad]
    public static class CameraAlignTool
    {
        static CameraAlignTool()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            if (_alignMain)
            {
                AlignMainCameraWithScene();
            }
            else if (_alignView)
            {
                AlignSceneCameraWithGame();
            }
        }

        static bool _alignMain;
        static bool _alignView;
        const string path1 = "Edit/Keep Aligning Main Camera With Scene View";
        const string path2 = "Edit/Keep Aligning Scene View With Main Camera";

        [MenuItem(path1, false, 21)]
        static void ToggleAlignMainCameraWithScene()
        {
            _alignMain = !_alignMain;
            Menu.SetChecked(path1, _alignMain);
        }

        [MenuItem(path2, false, 22)]
        static void ToggleAlignSceneCameraWithGame()
        {
            _alignView = !_alignView;
            Menu.SetChecked(path2, _alignView);
        }

        [MenuItem("Edit/Align Main Camera With Scene View", false, 23)]
        static void AlignMainCameraWithScene()
        {
            var sceneViewCamera = SceneView.lastActiveSceneView.camera;
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.transform.position = sceneViewCamera.transform.position;
                mainCamera.transform.rotation = sceneViewCamera.transform.rotation;
            }
        }

        [MenuItem("Edit/Align Scene Camera With Game View", false, 24)]
        static void AlignSceneCameraWithGame()
        {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.orthographic = Camera.main.orthographic;
            sceneView.LookAtDirect(Camera.main.transform.position, Camera.main.transform.rotation, 0);
        }
    }
}
