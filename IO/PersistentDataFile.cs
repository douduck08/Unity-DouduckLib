using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DouduckLib.IO {
	public class PersistentDataFile {

		public static string ReadToEnd (string filePath) {
            filePath = Path.Combine (Application.persistentDataPath, filePath);
            StreamReader sr_ = null;
            try {
                sr_ = File.OpenText (filePath);
            } catch (System.Exception ex) {
                Debug.LogErrorFormat ("[PersistentDataFile] Failed to load `{0}`, error: {1}", filePath, ex);
                return null;
            }
            string text_ = sr_.ReadToEnd ();
            sr_.Close ();
            sr_.Dispose ();
            return text_;
        }

		public static void WriteText (string filePath, string text, bool append = false) {
            filePath = Path.Combine (Application.persistentDataPath, filePath);
            StreamWriter sw_;
            FileInfo file_ = new FileInfo (filePath);
            if (!file_.Exists) {
                sw_ = file_.CreateText ();
            } else {
				if (!append) {
            		File.Delete (filePath);
				}
                sw_ = file_.CreateText ();
            }
            sw_.WriteLine (text);
            sw_.Close ();
            sw_.Dispose ();
        }
	}
}
