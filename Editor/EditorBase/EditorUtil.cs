using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class EditorUtil {
        public const string MenuItemPathRoot = "Tools/DouduckLib/";

        public static void SaveAsPNG (Texture2D texture, string defaultName) {
            if (texture == null) {
                throw new System.ArgumentNullException ("texture is null");
            }

            string path = EditorUtility.SaveFilePanel ("Save as PNG", Application.dataPath, defaultName, "png");
            if (path.Length != 0) {
                byte[] bytes = texture.EncodeToPNG ();
                File.WriteAllBytes (path, bytes);
            }
        }

        public static void SaveAsJPG (Texture2D texture, string defaultName) {
            if (texture == null) {
                throw new System.ArgumentNullException ("texture is null");
            }

            string path = EditorUtility.SaveFilePanel ("Save as JPG", Application.dataPath, defaultName, "jpg");
            if (path.Length != 0) {
                byte[] bytes = texture.EncodeToJPG ();
                File.WriteAllBytes (path, bytes);
            }
        }
    }
}