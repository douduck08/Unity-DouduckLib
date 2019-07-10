using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    [InitializeOnLoad]
    public class CameraAlignTool {

        static CameraAlignTool () {
            EditorApplication.update += Update;
        }

        static void Update () {
            if (alignToggle) {
                AlignMainCameraWithScene ();
            }
        }

        static bool alignToggle;
        const string path = "Edit/Keep Aligning Main Camera With Scene View";

        [MenuItem (path, false, 21)]
        static void ToggleAlignMainCameraWithScene () {
            alignToggle = !alignToggle;
            Menu.SetChecked (path, alignToggle);
        }

        [MenuItem ("Edit/Align Main Camera With Scene View", false, 22)]
        static void AlignMainCameraWithScene () {
            var sceneViewCamera = SceneView.lastActiveSceneView.camera;
            var mainCamera = Camera.main;
            if (mainCamera != null) {
                mainCamera.transform.position = sceneViewCamera.transform.position;
                mainCamera.transform.rotation = sceneViewCamera.transform.rotation;
            }
        }

        [MenuItem ("Edit/Align Scene Camera With Game View", false, 23)]
        static void AlignSceneCameraWithGame () {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.orthographic = Camera.main.orthographic;
            sceneView.LookAtDirect (Camera.main.transform.position, Camera.main.transform.rotation, 0);
        }
    }
}