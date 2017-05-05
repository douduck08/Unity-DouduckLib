using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DouduckGame.Util {
    public enum ResourceStatus {
        LOAD_SUCCESS,
        LOAD_FAILURE,
        LOAD_FILE_END,
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    class ResourceTool {
        public static void LoadText (string filePath, System.Action<ResourceStatus, string> callback) {
            filePath = Application.dataPath + "/Resources/" + filePath;
            StreamReader sr_ = File.OpenText (filePath);
            if (sr_ == null) {
                callback (ResourceStatus.LOAD_FAILURE, "");
                return;
            }
            callback (ResourceStatus.LOAD_SUCCESS, sr_.ReadToEnd ());
            sr_.Close ();
            sr_.Dispose ();
        }

        public static void LoadTextRows (string filePath, System.Action<ResourceStatus, int, string> callback) {
            filePath = Application.dataPath + "/Resources/" + filePath;
            StreamReader sr_ = File.OpenText (filePath);
            if (sr_ == null) {
                callback (ResourceStatus.LOAD_FAILURE, -1, "");
                return;
            }
            int lineNum = 0;
            while (!sr_.EndOfStream) {
                callback (ResourceStatus.LOAD_SUCCESS, lineNum, sr_.ReadLine ());
                lineNum += 1;
            }
            callback (ResourceStatus.LOAD_FILE_END, lineNum, "");
            sr_.Close ();
            sr_.Dispose ();
        }
    }

#elif UNITY_ANDROID || UNITY_IPHONE
    class ResourceTool {
        public static void LoadText (string filePath, System.Action<ResourceStatus, string> callback) {
            TextAsset logFile = Resources.Load<TextAsset> (Path.GetFileNameWithoutExtension (filePath));
            StringReader sr_ = new StringReader (logFile.text);
            if (sr_ == null) {
                callback (ResourceStatus.LOAD_FAILURE, "");
                return;
            }
            callback (ResourceStatus.LOAD_SUCCESS, sr_.ReadToEnd ());
            sr_.Close ();
            sr_.Dispose ();
        }

        public static void LoadTextRows (string filePath, System.Action<ResourceStatus, int, string> callback) {
            TextAsset logFile = Resources.Load<TextAsset> (Path.GetFileNameWithoutExtension (filePath));
            StringReader sr_ = new StringReader (logFile.text);
            if (sr_ == null) {
                callback (ResourceStatus.LOAD_FAILURE, -1, "");
                return;
            }
            int lineNum = 0;
            string row;
            while ((row = sr_.ReadLine ()) != null) {
                callback (ResourceStatus.LOAD_SUCCESS, lineNum, row);
                lineNum += 1;
            }
            callback (ResourceStatus.LOAD_FILE_END, lineNum, "");
            sr_.Close ();
            sr_.Dispose ();
        }
    }
#endif
}