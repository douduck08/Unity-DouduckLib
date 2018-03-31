using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using MEC;

namespace DouduckLib.IO {
    public class StreamingAssetsFile {

#if UNITY_STANDALONE || UNITY_EDITOR
        static readonly string localhostPath = "file:///" + Application.streamingAssetsPath;
#elif UNITY_ANDROID || UNITY_IOS
        static readonly string localhostPath = Application.streamingAssetsPath;
#endif
        private static string ParseUrl (string url) {
            if (url.Contains ("://")) {
                return url;
            }
            return Path.Combine (localhostPath, url);
        }

        public static void LoadText (string url, Action<string> callback) {
            Timing.RunCoroutine (StartLoadTextRows (ParseUrl(url), callback));
        }

        public static void LoadText (string url, Action<IEnumerable<string>> callback) {
            Timing.RunCoroutine (StartLoadTextRows (ParseUrl(url), (text) => {
                StringReader sr_ = new StringReader (text);
                List<string> rows = new List<string> ();
                string row;
                while ((row = sr_.ReadLine ()) != null) {
                    rows.Add(row);
                }
                sr_.Close ();
                sr_.Dispose ();
                callback (rows);
            }));
        }

        private static IEnumerator<float> StartLoadTextRows (string url, Action<string> callback) {
            WWW www = new WWW (url);
            yield return Timing.WaitUntilDone (www);
            if (string.IsNullOrEmpty (www.error)) {
                callback (www.text);
            } else {
                Debug.LogErrorFormat ("[StreamingAssetsFile] Failed to load `{0}`, error: {1}", url, www.error);
                callback ("");
            }
        }
    }
}