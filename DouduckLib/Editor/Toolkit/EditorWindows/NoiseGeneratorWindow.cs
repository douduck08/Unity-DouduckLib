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

        bool fold;

        protected override void OnDrawGUIBody () {
            EditorGUIWrapper.DrawSection ("Section", () => {
                using (new EditorGUILayout.HorizontalScope ()) {
                    EditorGUIWrapper.DrawTexturePreview (Texture2D.whiteTexture, new Vector2 (100, 100));
                    EditorGUIWrapper.DrawTexturePreview (Texture2D.whiteTexture, new Vector2 (100, 100));
                    EditorGUIWrapper.DrawTexturePreview (Texture2D.whiteTexture, new Vector2 (100, 100));
                }
                if (GUILayout.Button ("Button", EditorGUIStyle.Button)) {

                }
            });

            EditorGUIWrapper.DrawBox (() => {

                if (GUILayout.Button ("Button", EditorGUIStyle.Button)) {

                }

                if (GUILayout.Button ("Button", EditorGUIStyle.Button)) {

                }
            });
        }

    }
}