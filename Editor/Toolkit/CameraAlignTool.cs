using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor
{
    [InitializeOnLoad]
    public class CameraAlignTool
    {

        static CameraAlignTool()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            if (alignMain)
            {
                AlignMainCameraWithScene();
            }
            else if (alignView)
            {
                AlignSceneCameraWithGame();
            }
        }

        static bool alignMain;
        static bool alignView;
        const string path1 = "Edit/Keep Aligning Main Camera With Scene View";
        const string path2 = "Edit/Keep Aligning Scene View With Main Camera";

        [MenuItem(path1, false, 21)]
        static void ToggleAlignMainCameraWithScene()
        {
            alignMain = !alignMain;
            Menu.SetChecked(path2, alignMain);
        }

        [MenuItem(path2, false, 22)]
        static void ToggleAlignSceneCameraWithGame()
        {
            alignView = !alignView;
            Menu.SetChecked(path2, alignView);
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
