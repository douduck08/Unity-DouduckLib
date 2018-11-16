using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class CaptureScreenshotTool {

        readonly static string screenshotFilePath = "../Screenshot";

        [MenuItem ("Edit/CaptureScreenshot %k")]
        public static void CaptureScreenshot () {
            string path = Path.Combine (Application.dataPath, screenshotFilePath);
            if (!Directory.Exists (path)) {
                Directory.CreateDirectory (path);
            }
            path = Path.Combine (path, DateTime.Now.ToString ("yyyyMMdd-HHmmss") + ".png");
#if UNITY_2017_3_OR_NEWER
            ScreenCapture.CaptureScreenshot (path);
#else
            Application.CaptureScreenshot (path);
#endif
        }
    }
}