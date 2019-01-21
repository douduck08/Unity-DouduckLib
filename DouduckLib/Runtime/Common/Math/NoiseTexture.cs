using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public enum NoiseType {
        Value,
        Perlin,
    }

    public static class NoiseTexture {

        static Gradient coloring = new Gradient () {
            colorKeys = new GradientColorKey[] { new GradientColorKey (Color.black, 0), new GradientColorKey (Color.white, 1) }
        };

        public static Texture2D CreateNoiseTexture2D (int resolution, float scale = 1f, NoiseType noiseType = NoiseType.Perlin, int octaves = 1, float persistence = 0.5f, float lacunarity = 2f) {
            return CreateNoiseTexture2D (resolution, coloring, scale, noiseType, octaves, persistence, lacunarity);
        }

        public static Texture2D CreateNoiseTexture2D (int resolution, Gradient coloring, float scale = 1f, NoiseType noiseType = NoiseType.Perlin, int octaves = 1, float persistence = 0.5f, float lacunarity = 2f) {
            Texture2D texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, false);
            texture.name = "Procedural Texture";
            texture.wrapMode = TextureWrapMode.Clamp;

            float texelSize = 1f / resolution;
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    var point = new Vector2 ((x + 0.5f) * texelSize, (y + 0.5f) * texelSize);
                    float noise = 0f;
                    if (noiseType == NoiseType.Value) {
                        noise = Octaves (Noise.Value2D, point, scale, octaves, lacunarity, persistence);
                    } else {
                        noise = Octaves (Noise.Perlin2D, point, scale, octaves, lacunarity, persistence);
                    }
                    texture.SetPixel (x, y, coloring.Evaluate (noise));
                }
            }
            texture.Apply ();

            return texture;
        }

        static float Octaves (System.Func<Vector2, float, float> method, Vector2 point, float scale, int octaves, float persistence, float lacunarity) {
            float sum = method (point, scale);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++) {
                scale *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += method (point, scale) * amplitude;
            }
            return sum / range;
        }
    }
}