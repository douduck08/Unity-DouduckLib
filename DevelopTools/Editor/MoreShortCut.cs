using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class MoreShortCut {

        readonly static string screenshotFilePath = "../Bin/PrintScreen";

        [MenuItem ("Edit/Lock %l")]
        public static void LockInspector () {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild ();
        }

        [MenuItem ("Edit/Lock %l", true)]
        public static bool LockInspectorValid () {
            return ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
        }

        [MenuItem ("Edit/CaptureScreenshot %k")]
        public static void CaptureScreenshot () {
            string path = Path.Combine (Application.dataPath, screenshotFilePath);
            if (!Directory.Exists (path)) {
                Directory.CreateDirectory (path);
            }
            path = Path.Combine (path, DateTime.Now.ToString ("yyyyMMdd-HHmmss") + ".png");
            ScreenCapture.CaptureScreenshot (path);
        }
    }
}