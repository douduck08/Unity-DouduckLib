using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DouduckLib;

namespace DouduckLibEditor
{
    public class NoiseGeneratorWindow : EditorWindowBase<NoiseGeneratorWindow>
    {

        [MenuItem(EditorUtil.MenuItemPathRoot + "Noise Texture Generator", false, 20)]
        public static void OpenWindow()
        {
            Open("Noise Texture Generator");
        }

        NoiseType noiseType = NoiseType.Perlin;
        Gradient coloring = new Gradient()
        {
            colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) }
        };
        float scale = 16f;
        int octaves = 1;
        float persistence = 0.5f;
        float lacunarity = 2f;

        int resolution = 512;
        Texture2D preview;

        protected override void OnDrawGUIBody()
        {
            EditorGUIWrapper.DrawSection("Noise Settings", () =>
            {
                noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise Type", noiseType);
                coloring = EditorGUIWrapper.DrawGradientField("Coloring", coloring);
                scale = EditorGUILayout.Slider("Scale", scale, 1f, 100f);
                octaves = EditorGUILayout.IntSlider("Octaves", octaves, 1, 8);
                persistence = EditorGUILayout.Slider("Persistence", persistence, 0f, 1f);
                lacunarity = EditorGUILayout.Slider("Lacunarity", lacunarity, 1f, 4f);
                if (GUILayout.Button("Preview"))
                {
                    preview = NoiseTexture.CreateNoiseTexture2D(256, coloring, scale, noiseType, octaves, persistence, lacunarity);
                }
            });

            EditorGUIWrapper.DrawSection("Preview", () =>
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (preview == null)
                        preview = Texture2D.whiteTexture;
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
                        var tmp = NoiseTexture.CreateNoiseTexture2D(resolution, coloring, scale, noiseType, octaves, persistence, lacunarity);
                        EditorUtil.SaveAsPNG(tmp, "noise texture");
                    }
                    if (GUILayout.Button("Export JPG"))
                    {
                        var tmp = NoiseTexture.CreateNoiseTexture2D(resolution, coloring, scale, noiseType, octaves, persistence, lacunarity);
                        EditorUtil.SaveAsJPG(tmp, "noise texture");
                    }
                }
            });
        }
    }
}
