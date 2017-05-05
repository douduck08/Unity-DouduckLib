using System.Collections;
using System.IO;
using UnityEngine;

namespace DouduckGame.Util {
    class PersistentDataTool {

        public static string ReadText (string sPath, string sFileName) {
            sPath = Application.persistentDataPath + sPath;
            StreamReader sr_ = null;
            try {
                sr_ = File.OpenText (sPath + "//" + sFileName);
            } catch (System.Exception ex) {
                UnityConsole.LogWarning ("[PersistentDataTool] Load error: " + ex);
                return null;
            }
            string sText_ = sr_.ReadToEnd ();
            sr_.Close ();
            sr_.Dispose ();
            return sText_;
        }

        public static void WriteText (string sPath, string sFileName, string sMessage) {
            sPath = Application.persistentDataPath + sPath;
            StreamWriter sw_;
            FileInfo file_ = new FileInfo (sPath + "//" + sFileName);
            if (!file_.Exists) {
                sw_ = file_.CreateText ();
            } else {
                DeleteFile (sPath, sFileName);
                sw_ = file_.CreateText ();
            }
            sw_.WriteLine (sMessage);
            sw_.Close ();
            sw_.Dispose ();
        }

        public static void AppendText (string sPath, string sFileName, string sMessage) {
            sPath = Application.persistentDataPath + sPath;
            StreamWriter sw_;
            FileInfo file_ = new FileInfo (sPath + "//" + sFileName);
            if (!file_.Exists) {
                sw_ = file_.CreateText ();
            } else {
                sw_ = file_.AppendText ();
            }
            sw_.WriteLine (sMessage);
            sw_.Close ();
            sw_.Dispose ();
        }

        public static void DeleteFile (string sPath, string sFileName) {
            sPath = Application.persistentDataPath + sPath;
            File.Delete (sPath + "//" + sFileName);
        }
    }
}
