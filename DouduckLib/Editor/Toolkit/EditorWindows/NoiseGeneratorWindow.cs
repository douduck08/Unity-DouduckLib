using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor {
    public class NoiseGeneratorWindow : EditorWindowBase<NoiseGeneratorWindow> {

        [MenuItem (EditorUtil.MenuItemPathRoot + "Test Noise Generator", false, 20)]
        public static void OpenWindow () {
            Open ("Noise Generator");
        }

        void OnGUI () {
            GUILayout.Label (title);
        }
    }
}