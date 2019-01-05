using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorGUIWrapper {

    public static void DrawHorizontalLine () {
        GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
    }

    public static void DrawHeader (string title) {
        GUILayout.Label (title, EditorGUIStyle.Header);
    }

    public static void DrawBox (Action onGUI) {
        using (new EditorGUILayout.VerticalScope (EditorGUIStyle.Box)) {
            // GUILayout.FlexibleSpace ();
            if (onGUI != null) onGUI.Invoke ();
            // GUILayout.FlexibleSpace ();
        }
    }

    public static void DrawSection (string title, Action onGUI) {
        using (new EditorGUILayout.VerticalScope (EditorGUIStyle.Box)) {
            GUILayout.Label (title, EditorGUIStyle.Section);

            // GUILayout.FlexibleSpace ();
            if (onGUI != null) onGUI.Invoke ();
            // GUILayout.FlexibleSpace ();
        }
    }

    // TODO: Foldout Section
    // public static bool DrawFoldoutSection (bool fold, string title, Action onGUI) {
    //     using (new EditorGUILayout.VerticalScope (EditorGUIStyle.Box)) {
    //         using (new EditorGUILayout.HorizontalScope ()) {
    //             var foldStyle = new GUIStyle ("Foldout");
    //             foldStyle.fixedWidth = 1;
    //             fold = EditorGUILayout.Foldout (fold, "", foldStyle);
    //             GUILayout.Label (title, EditorGUIStyle.Section);
    //         }

    //         // GUILayout.FlexibleSpace ();
    //         if (fold && onGUI != null) onGUI.Invoke ();
    //         // GUILayout.FlexibleSpace ();
    //     }
    //     return fold;
    // }

    public static void DrawTexturePreview (Texture2D texture, Vector2 previewSize) {
        using (new EditorGUILayout.HorizontalScope ()) {
            GUILayout.FlexibleSpace ();

            GUILayout.Label ("", EditorGUIStyle.Box, GUILayout.Width (previewSize.x), GUILayout.Height (previewSize.y));

            Rect previewRect = GUILayoutUtility.GetLastRect ();
            Rect alphaRect = new Rect (previewRect.x + 5, previewRect.y + 5, previewRect.width - 10, previewRect.height - 10);
            EditorGUI.DrawPreviewTexture (alphaRect, texture);

            GUILayout.FlexibleSpace ();
        }
    }
}