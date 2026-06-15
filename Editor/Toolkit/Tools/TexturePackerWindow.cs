using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DouduckLibEditor
{
    public class TexturePackerWindow : EditorWindowBase<TexturePackerWindow>
    {
        [MenuItem(EditorUtil.MenuItemPathRoot + "Texture Packer", false, 20)]
        public static void OpenWindow()
        {
            Open("Texture Packer");
        }

        public enum Channel
        {
            Red,
            Green,
            Blue,
            Alpha
        }

        const string _shaderName = "Hidden/TexturePacker";

        Material _material;
        int _resolution = 512;
        Texture2D _preview;

        Texture2D _textureR;
        Texture2D _textureG;
        Texture2D _textureB;
        Texture2D _textureA;
        Channel _channelR;
        Channel _channelG;
        Channel _channelB;
        Channel _channelA;

        bool _foldoutR;
        bool _foldoutG;
        bool _foldoutB;
        bool _foldoutA;

        protected override void OnDrawGUIBody()
        {
            EditorGUIWrapper.DrawSection("Noise Settings", () =>
            {
                _foldoutR = EditorGUIWrapper.DrawFoldout(_foldoutR, "Red Channel", () =>
                {
                    _textureR = (Texture2D)EditorGUILayout.ObjectField("Source Texture", _textureR, typeof(Texture2D), false);
                    _channelR = (Channel)EditorGUILayout.EnumPopup("Selected Channel", _channelR);
                });
                _foldoutG = EditorGUIWrapper.DrawFoldout(_foldoutG, "Green Channel", () =>
                {
                    _textureG = (Texture2D)EditorGUILayout.ObjectField("Source Texture", _textureG, typeof(Texture2D), false);
                    _channelG = (Channel)EditorGUILayout.EnumPopup("Selected Channel", _channelG);
                });
                _foldoutB = EditorGUIWrapper.DrawFoldout(_foldoutB, "Blue Channel", () =>
                {
                    _textureB = (Texture2D)EditorGUILayout.ObjectField("Source Texture", _textureB, typeof(Texture2D), false);
                    _channelB = (Channel)EditorGUILayout.EnumPopup("Selected Channel", _channelB);
                });
                _foldoutA = EditorGUIWrapper.DrawFoldout(_foldoutA, "Alpha Channel", () =>
                {
                    _textureA = (Texture2D)EditorGUILayout.ObjectField("Source Texture", _textureA, typeof(Texture2D), false);
                    _channelA = (Channel)EditorGUILayout.EnumPopup("Selected Channel", _channelA);
                });
                if (GUILayout.Button("Preview"))
                {
                    _preview = PackageTexture(256);
                }
            });

            EditorGUIWrapper.DrawSection("Preview", () =>
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (_preview == null)
                        _preview = PackageTexture(256);
                    EditorGUIWrapper.DrawTexturePreview(_preview, new Vector2(256, 256));
                }
            });

            EditorGUIWrapper.DrawSection("Export", () =>
            {
                _resolution = EditorGUILayout.IntField("Resolution", _resolution);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Export PNG"))
                    {
                        var tmp = PackageTexture(_resolution);
                        EditorUtil.SaveAsPNG(tmp, "packaged texture");
                    }
                    if (GUILayout.Button("Export JPG"))
                    {
                        var tmp = PackageTexture(_resolution);
                        EditorUtil.SaveAsJPG(tmp, "packaged texture");
                    }
                }
            });
        }

        Texture2D PackageTexture(int resolution)
        {
            if (_material == null)
                _material = new Material(Shader.Find(_shaderName));

            _material.SetTexture("_TextureR", _textureR);
            _material.SetTexture("_TextureG", _textureG);
            _material.SetTexture("_TextureB", _textureB);
            _material.SetTexture("_TextureA", _textureA);
            _material.SetInt("_ChannelR", (int)_channelR);
            _material.SetInt("_ChannelG", (int)_channelG);
            _material.SetInt("_ChannelB", (int)_channelB);
            _material.SetInt("_ChannelA", (int)_channelA);

            RenderTexture tempRT = RenderTexture.GetTemporary(resolution, resolution);
            Graphics.Blit(null, tempRT, _material);
            RenderTexture.active = tempRT;

            Texture2D result = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            result.name = "Procedural Texture";
            result.wrapMode = TextureWrapMode.Clamp;
            result.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            result.Apply();
            result.filterMode = FilterMode.Bilinear;

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(tempRT);

            return result;
        }
    }
}
