using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class EditorShortCut {

        [MenuItem ("Edit/Lock %l")]
        public static void LockInspector () {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild ();
        }

        [MenuItem ("Edit/Lock %l", true)]
        public static bool LockInspectorValid () {
            return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
        }

        [MenuItem ("Edit/Unselect All %#a", false, 20)]
        public static void Unselect () {
            Selection.activeGameObject = null;
        }

        [MenuItem ("Edit/Set Camera As SceneView", false, 21)]
        public static void SetCameraAsSceneView () {
            var sceneViewCamera = SceneView.lastActiveSceneView.camera;
            Camera.main.transform.position = sceneViewCamera.transform.position;
            Camera.main.transform.rotation = sceneViewCamera.transform.rotation;
        }

        [MenuItem ("Edit/Set Camera As GameView", false, 22)]
        public static void SetCameraAsGameView () {
            var sceneView = SceneView.lastActiveSceneView;
            sceneView.orthographic = Camera.main.orthographic;
            sceneView.LookAtDirect (Camera.main.transform.position, Camera.main.transform.rotation, 0);
        }
    }
}