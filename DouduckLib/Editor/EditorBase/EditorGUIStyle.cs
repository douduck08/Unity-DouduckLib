using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorGUIStyle {

    static GUIStyle header = null;
    public static GUIStyle Header {
        get {
            if (header == null) {
                header = new GUIStyle ("OL Title");
                header.margin = new RectOffset (4, 4, 4, 4);
                header.padding = new RectOffset (8, 8, 8, 8);
                header.fixedHeight = 0;

                header.fontSize = 14;
                header.alignment = TextAnchor.MiddleCenter;
            }
            return header;
        }
    }

    static GUIStyle box;
    public static GUIStyle Box {
        get {
            if (box == null) {
                box = new GUIStyle (GUI.skin.GetStyle ("OL Box"));
                box.margin = new RectOffset (4, 4, 0, 4);
                box.padding = new RectOffset (5, 5, 5, 5);
                box.stretchHeight = false;
                box.stretchWidth = true;
                // box.normal.textColor = new Color (0.7f, 0.7f, 0.7f, 1);
                box.fontSize = 10;
                box.alignment = TextAnchor.MiddleCenter;
            }
            return box;
        }
    }

    static GUIStyle section;
    public static GUIStyle Section {
        get {
            if (section == null) {
                section = new GUIStyle (GUI.skin.GetStyle ("OL Box"));
                section.padding = new RectOffset (4, 4, 4, 4);
                section.margin = new RectOffset (4, 4, 4, 4);
                section.stretchHeight = false;
                section.stretchWidth = true;
                // section.normal.textColor = new Color (0.7f, 0.7f, 0.7f, 1);
            }
            return section;
        }
    }

    static GUIStyle flodout;
    public static GUIStyle Flodout {
        get {
            if (flodout == null) {
                flodout = new GUIStyle (EditorStyles.foldout);
                flodout.padding = new RectOffset (18, 4, 0, 4);
                flodout.margin = new RectOffset (4, 4, 4, 4);
                flodout.stretchHeight = false;
                flodout.stretchWidth = true;

                // flodout.normal.textColor = new Color (0.7f, 0.7f, 0.7f, 1);
                flodout.active.textColor = flodout.normal.textColor;
                flodout.onActive.textColor = flodout.normal.textColor;
                flodout.focused.textColor = flodout.normal.textColor;
                flodout.onFocused.textColor = flodout.normal.textColor;
            }
            return flodout;
        }
    }

    static GUIStyle button;
    public static GUIStyle Button {
        get {
            if (button == null) {
                button = new GUIStyle (GUI.skin.button);
                button.border = new RectOffset (3, 3, 3, 3);
                button.padding = new RectOffset (1, 1, 15, 15);
                button.margin = new RectOffset (5, 5, 5, 5);
                button.contentOffset = Vector2.zero;
                button.fontSize = 12;
                // button.normal.textColor = new Color (0.7f, 0.7f, 0.7f, 1);
                button.alignment = TextAnchor.MiddleCenter;
            }
            return button;
        }
    }

    static Texture2D GetBorderedTexture (Color border, Color center) {
        Texture2D returnTex = new Texture2D (3, 3);
        returnTex.filterMode = FilterMode.Point;
        for (int x = 0; x < returnTex.width; x++) {
            for (int y = 0; y < returnTex.height; y++) {
                returnTex.SetPixel (x, y, border);
            }
        }
        returnTex.SetPixel (1, 1, center);
        returnTex.Apply ();
        returnTex.hideFlags = HideFlags.HideAndDontSave;
        return returnTex;
    }
}