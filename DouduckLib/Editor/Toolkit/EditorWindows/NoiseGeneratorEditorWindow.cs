using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DouduckLib;

namespace DouduckLibEditor {
    public class NoiseGeneratorEditorWindow : EditorWindow {

        static NoiseGenerator s_noiseGenerator;

        [SerializeField]
        NoiseGenerator noiseGenerator;
        [SerializeField]
        Texture2D noiseTexture;
        int exportResolution = 512;

        SerializedObject serializedObject;
        SerializedProperty generatorProperty;
        // SerializedProperty textureProperty;

        [MenuItem (EditorUtil.MenuItemPathRoot + "Noise Generator", false, 20)]
        public static void ShowWindow () {
            EditorWindow.GetWindow (typeof (NoiseGeneratorEditorWindow), false, "Noise Generator");
        }

        void OnFocus () {
            if (noiseGenerator == null) {
                if (s_noiseGenerator == null) {
                    s_noiseGenerator = new NoiseGenerator ();
                }
                noiseGenerator = s_noiseGenerator;
                noiseTexture = noiseGenerator.CreateTexture (256);

                serializedObject = new SerializedObject (this);
                generatorProperty = serializedObject.FindProperty ("noiseGenerator");
                // textureProperty = serializedObject.FindProperty ("noiseTexture");
            }
        }

        void OnGUI () {
            EditorGUI.BeginChangeCheck ();
            EditorGUILayout.PropertyField (generatorProperty, true);
            if (EditorGUI.EndChangeCheck ()) {
                serializedObject.ApplyModifiedProperties ();
                noiseGenerator.Apply (noiseTexture);
            }

            EditorGUILayout.BeginHorizontal ();
            exportResolution = EditorGUILayout.IntField ("Export Resolution", exportResolution);
            if (GUILayout.Button ("Export PNG")) {
                var tmp = noiseGenerator.CreateTexture (exportResolution);
                EditorUtil.SaveAsPNG (tmp, "new noise texture");
            }
            if (GUILayout.Button ("Export JPG")) {
                var tmp = noiseGenerator.CreateTexture (exportResolution);
                EditorUtil.SaveAsJPG (tmp, "new noise texture");
            }
            EditorGUILayout.EndHorizontal ();

            GUI.enabled = false;
            var resolution = Mathf.Min (position.width, position.height - EditorGUI.GetPropertyHeight (generatorProperty, true)) - 24f;
            EditorGUILayout.ObjectField (noiseTexture, typeof (Texture2D), false, GUILayout.Height (resolution), GUILayout.Width (resolution));
            GUI.enabled = true;
        }
    }
}