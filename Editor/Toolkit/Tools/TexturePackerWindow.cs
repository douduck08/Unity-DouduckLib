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

        const string shaderName = "Hidden/TexturePacker";

        Material material;
        int resolution = 512;
        Texture2D preview;

        Texture2D textureR;
        Texture2D textureG;
        Texture2D textureB;
        Texture2D textureA;
        Channel channelR;
        Channel channelG;
        Channel channelB;
        Channel channelA;

        bool foldoutR;
        bool foldoutG;
        bool foldoutB;
        bool foldoutA;

        protected override void OnDrawGUIBody()
        {
            EditorGUIWrapper.DrawSection("Noise Settings", () =>
            {
                foldoutR = EditorGUIWrapper.DrawFoldout(foldoutR, "Red Channel", () =>
                {
                    textureR = (Texture2D)EditorGUILayout.ObjectField("Source Texture", textureR, typeof(Texture2D), false);
                    channelR = (Channel)EditorGUILayout.EnumPopup("Selected Channel", channelR);
                });
                foldoutG = EditorGUIWrapper.DrawFoldout(foldoutG, "Green Channel", () =>
                {
                    textureG = (Texture2D)EditorGUILayout.ObjectField("Source Texture", textureG, typeof(Texture2D), false);
                    channelG = (Channel)EditorGUILayout.EnumPopup("Selected Channel", channelG);
                });
                foldoutB = EditorGUIWrapper.DrawFoldout(foldoutB, "Blue Channel", () =>
                {
                    textureB = (Texture2D)EditorGUILayout.ObjectField("Source Texture", textureB, typeof(Texture2D), false);
                    channelB = (Channel)EditorGUILayout.EnumPopup("Selected Channel", channelB);
                });
                foldoutA = EditorGUIWrapper.DrawFoldout(foldoutA, "Alpha Channel", () =>
                {
                    textureA = (Texture2D)EditorGUILayout.ObjectField("Source Texture", textureA, typeof(Texture2D), false);
                    channelA = (Channel)EditorGUILayout.EnumPopup("Selected Channel", channelA);
                });
                if (GUILayout.Button("Preview"))
                {
                    preview = PackageTexture(256);
                }
            });

            EditorGUIWrapper.DrawSection("Preview", () =>
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (preview == null)
                        preview = PackageTexture(256);
                    EditorGUIWrapper.DrawTexturePreview(preview, new Vector2(256, 256));
                }
            });

            EditorGUIWrapper.DrawSection("Export", () =>
            {
                resolution = EditorGUILayout.IntField("Resolution", resolution);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Export PNG"))
                    {
                        var tmp = PackageTexture(resolution);
                        EditorUtil.SaveAsPNG(tmp, "packaged texture");
                    }
                    if (GUILayout.Button("Export JPG"))
                    {
                        var tmp = PackageTexture(resolution);
                        EditorUtil.SaveAsJPG(tmp, "packaged texture");
                    }
                }
            });
        }

        Texture2D PackageTexture(int resolution)
        {
            if (material == null)
                material = new Material(Shader.Find(shaderName));

            material.SetTexture("_TextureR", textureR);
            material.SetTexture("_TextureG", textureG);
            material.SetTexture("_TextureB", textureB);
            material.SetTexture("_TextureA", textureA);
            material.SetInt("_ChannelR", (int)channelR);
            material.SetInt("_ChannelG", (int)channelG);
            material.SetInt("_ChannelB", (int)channelB);
            material.SetInt("_ChannelA", (int)channelA);

            RenderTexture tempRT = RenderTexture.GetTemporary(resolution, resolution);
            Graphics.Blit(null, tempRT, material);
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
