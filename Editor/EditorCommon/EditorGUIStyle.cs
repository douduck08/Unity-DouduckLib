using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor
{
    public static class EditorGUIStyle
    {
        static GUIStyle _header = null;
        public static GUIStyle Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new GUIStyle("OL Title");
                    _header.margin = new RectOffset(4, 4, 4, 4);
                    _header.padding = new RectOffset(8, 8, 8, 8);
                    _header.fixedHeight = 0;

                    _header.fontSize = 14;
                    _header.alignment = TextAnchor.MiddleCenter;
                }
                return _header;
            }
        }

        static GUIStyle _box;
        public static GUIStyle Box
        {
            get
            {
                if (_box == null)
                {
                    _box = new GUIStyle(GUI.skin.GetStyle("OL Box"));
                    _box.margin = new RectOffset(4, 4, 0, 4);
                    _box.padding = new RectOffset(5, 5, 5, 5);
                    _box.stretchHeight = false;
                    _box.stretchWidth = true;

                    if (EditorGUIUtility.isProSkin)
                        _box.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
                    _box.fontSize = 10;
                    _box.alignment = TextAnchor.MiddleCenter;
                }
                return _box;
            }
        }

        static GUIStyle _section;
        public static GUIStyle Section
        {
            get
            {
                if (_section == null)
                {
                    _section = new GUIStyle(GUI.skin.GetStyle("OL Box"));
                    _section.padding = new RectOffset(4, 4, 4, 4);
                    _section.margin = new RectOffset(4, 4, 4, 4);
                    _section.stretchHeight = false;
                    _section.stretchWidth = true;

                    if (EditorGUIUtility.isProSkin)
                        _section.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
                }
                return _section;
            }
        }

        static GUIStyle _foldout;
        public static GUIStyle Foldout
        {
            get
            {
                if (_foldout == null)
                {
                    _foldout = new GUIStyle(EditorStyles.foldout);
                    _foldout.padding = new RectOffset(18, 4, 0, 4);
                    _foldout.margin = new RectOffset(4, 4, 4, 4);
                    _foldout.stretchHeight = false;
                    _foldout.stretchWidth = true;

                    if (EditorGUIUtility.isProSkin)
                        _foldout.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
                    _foldout.active.textColor = _foldout.normal.textColor;
                    _foldout.onActive.textColor = _foldout.normal.textColor;
                    _foldout.focused.textColor = _foldout.normal.textColor;
                    _foldout.onFocused.textColor = _foldout.normal.textColor;
                }
                return _foldout;
            }
        }

        static GUIStyle _button;
        public static GUIStyle Button
        {
            get
            {
                if (_button == null)
                {
                    _button = new GUIStyle(GUI.skin.button);
                    _button.border = new RectOffset(3, 3, 3, 3);
                    _button.padding = new RectOffset(1, 1, 15, 15);
                    _button.margin = new RectOffset(5, 5, 5, 5);
                    _button.contentOffset = Vector2.zero;
                    _button.fontSize = 12;
                    _button.alignment = TextAnchor.MiddleCenter;
                }
                return _button;
            }
        }

        static Texture2D GetBorderedTexture(Color border, Color center)
        {
            Texture2D returnTex = new Texture2D(3, 3);
            returnTex.filterMode = FilterMode.Point;
            for (int x = 0; x < returnTex.width; x++)
            {
                for (int y = 0; y < returnTex.height; y++)
                {
                    returnTex.SetPixel(x, y, border);
                }
            }
            returnTex.SetPixel(1, 1, center);
            returnTex.Apply();
            returnTex.hideFlags = HideFlags.HideAndDontSave;
            return returnTex;
        }
    }
}
