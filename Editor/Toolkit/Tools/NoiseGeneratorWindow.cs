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

        NoiseType _noiseType = NoiseType.Perlin;
        Gradient _coloring = new Gradient()
        {
            colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) }
        };
        float _scale = 16f;
        int _octaves = 1;
        float _persistence = 0.5f;
        float _lacunarity = 2f;

        int _resolution = 512;
        Texture2D _preview;

        protected override void OnDrawGUIBody()
        {
            EditorGUIWrapper.DrawSection("Noise Settings", () =>
            {
                _noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise Type", _noiseType);
                _coloring = EditorGUIWrapper.DrawGradientField("Coloring", _coloring);
                _scale = EditorGUILayout.Slider("Scale", _scale, 1f, 100f);
                _octaves = EditorGUILayout.IntSlider("Octaves", _octaves, 1, 8);
                _persistence = EditorGUILayout.Slider("Persistence", _persistence, 0f, 1f);
                _lacunarity = EditorGUILayout.Slider("Lacunarity", _lacunarity, 1f, 4f);
                if (GUILayout.Button("Preview"))
                {
                    _preview = NoiseTexture.CreateNoiseTexture2D(256, _coloring, _scale, _noiseType, _octaves, _persistence, _lacunarity);
                }
            });

            EditorGUIWrapper.DrawSection("Preview", () =>
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (_preview == null)
                        _preview = Texture2D.whiteTexture;
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
                        var tmp = NoiseTexture.CreateNoiseTexture2D(_resolution, _coloring, _scale, _noiseType, _octaves, _persistence, _lacunarity);
                        EditorUtil.SaveAsPNG(tmp, "noise texture");
                    }
                    if (GUILayout.Button("Export JPG"))
                    {
                        var tmp = NoiseTexture.CreateNoiseTexture2D(_resolution, _coloring, _scale, _noiseType, _octaves, _persistence, _lacunarity);
                        EditorUtil.SaveAsJPG(tmp, "noise texture");
                    }
                }
            });
        }
    }
}
