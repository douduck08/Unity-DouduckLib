using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    [System.Serializable]
    public class NoiseGenerator {

        static int[] hash = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
        static int hashMask = 255;

        static float[] gradients1D = { 1f, -1f };
        static int gradientsMask1D = 1;

        static Vector2[] gradients2D = {
            new Vector2 (1f, 0f),
            new Vector2 (-1f, 0f),
            new Vector2 (0f, 1f),
            new Vector2 (0f, -1f),
            new Vector2 (1f, 1f).normalized,
            new Vector2 (-1f, 1f).normalized,
            new Vector2 (1f, -1f).normalized,
            new Vector2 (-1f, -1f).normalized
        };
        static int gradientsMask2D = 7;
        static float sqr2 = Mathf.Sqrt (2f);

        static Vector3[] gradients3D = {
            new Vector3 (1f, 1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (1f, -1f, 0f),
            new Vector3 (-1f, -1f, 0f),
            new Vector3 (1f, 0f, 1f),
            new Vector3 (-1f, 0f, 1f),
            new Vector3 (1f, 0f, -1f),
            new Vector3 (-1f, 0f, -1f),
            new Vector3 (0f, 1f, 1f),
            new Vector3 (0f, -1f, 1f),
            new Vector3 (0f, 1f, -1f),
            new Vector3 (0f, -1f, -1f),
            new Vector3 (1f, 1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (0f, -1f, 1f),
            new Vector3 (0f, -1f, -1f)
        };
        static int gradientsMask3D = 15;

        public delegate float NoiseMethod (Vector3 point, float scale);
        static NoiseMethod[] noiseMethods = new NoiseMethod[] { Value1D, Value2D, Value3D, Perlin1D, Perlin2D, Perlin3D };

        public enum NoiseType {
            Value = 0,
            Perlin = 3,
        }

        [Header ("Base Settings")]
        public NoiseType noiseType = 0;
        [Range (1, 3)]
        public int dimensions = 2;
        [Range (1f, 100f)]
        public float scale = 16f;
        public Gradient coloring = new Gradient () {
            colorKeys = new GradientColorKey[] { new GradientColorKey (Color.black, 0), new GradientColorKey (Color.white, 1) }
        };

        [Header ("Octaves Settings")]
        [Range (1, 8)]
        public int octaves = 1;
        [Range (1f, 4f)]
        public float lacunarity = 2f;
        [Range (0f, 1f)]
        public float persistence = 0.5f;

        public Texture2D CreateTexture (int resolution) {
            Texture2D texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, false);
            texture.name = "Procedural Texture";
            texture.wrapMode = TextureWrapMode.Clamp;
            Apply (texture);
            return texture;
        }

        public void Apply (Texture2D texture) {
            float resolution = texture.width;
            float stepSize = 1f / resolution;
            NoiseMethod noiseMethod = noiseMethods[(int) noiseType + dimensions - 1];
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    Vector3 point = new Vector3 ((x + 0.5f) * stepSize, (y + 0.5f) * stepSize, 0f);
                    // float noise = noiseMethod (point, scale);
                    float noise = Octaves (noiseMethod, point, scale, octaves, lacunarity, persistence);
                    if (noiseType != NoiseType.Value) {
                        noise = noise * 0.5f + 0.5f;
                    }
                    // texture.SetPixel (x, y, Color.Lerp (backgroundColor, frontColor, noise));
                    texture.SetPixel (x, y, coloring.Evaluate (noise));
                }
            }
            texture.Apply ();
        }

        static float Smooth (float t) {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        static float Dot (Vector2 g, float x, float y) {
            return g.x * x + g.y * y;
        }

        static float Dot (Vector3 g, float x, float y, float z) {
            return g.x * x + g.y * y + g.z * z;
        }

        public static float Value1D (Vector3 point, float scale) {
            point *= scale;
            int x = Mathf.FloorToInt (point.x);
            int h0 = hash[x & hashMask];
            int h1 = hash[(x + 1) & hashMask];
            return Mathf.Lerp (h0, h1, Smooth (point.x - x)) * (1f / hashMask);
        }

        public static float Value2D (Vector3 point, float scale) {
            point *= scale;
            int x = Mathf.FloorToInt (point.x);
            int y = Mathf.FloorToInt (point.y);

            int h0 = hash[x & hashMask];
            int h1 = hash[(x + 1) & hashMask];
            int h00 = hash[(h0 + y) & hashMask];
            int h10 = hash[(h1 + y) & hashMask];
            int h01 = hash[(h0 + y + 1) & hashMask];
            int h11 = hash[(h1 + y + 1) & hashMask];

            float hx0 = Mathf.Lerp (h00, h10, Smooth (point.x - x));
            float hx1 = Mathf.Lerp (h01, h11, Smooth (point.x - x));
            return Mathf.Lerp (hx0, hx1, Smooth (point.y - y)) * (1f / hashMask);
        }

        public static float Value3D (Vector3 point, float scale) {
            point *= scale;
            int x = Mathf.FloorToInt (point.x);
            int y = Mathf.FloorToInt (point.y);
            int z = Mathf.FloorToInt (point.z);

            int h0 = hash[x & hashMask];
            int h1 = hash[(x + 1) & hashMask];
            int h00 = hash[(h0 + y) & hashMask];
            int h10 = hash[(h1 + y) & hashMask];
            int h01 = hash[(h0 + y + 1) & hashMask];
            int h11 = hash[(h1 + y + 1) & hashMask];
            int h000 = hash[(h00 + z) & hashMask];
            int h100 = hash[(h10 + z) & hashMask];
            int h010 = hash[(h01 + z) & hashMask];
            int h110 = hash[(h11 + z) & hashMask];
            int h001 = hash[(h00 + z + 1) & hashMask];
            int h101 = hash[(h10 + z + 1) & hashMask];
            int h011 = hash[(h01 + z + 1) & hashMask];
            int h111 = hash[(h11 + z + 1) & hashMask];

            float hx00 = Mathf.Lerp (h000, h100, Smooth (point.x - x));
            float hx10 = Mathf.Lerp (h010, h110, Smooth (point.x - x));
            float hx01 = Mathf.Lerp (h001, h101, Smooth (point.x - x));
            float hx11 = Mathf.Lerp (h011, h111, Smooth (point.x - x));
            float hxy0 = Mathf.Lerp (hx00, hx10, Smooth (point.y - y));
            float hxy1 = Mathf.Lerp (hx01, hx11, Smooth (point.y - y));
            return Mathf.Lerp (hxy0, hxy1, Smooth (point.z - z)) * (1f / hashMask);;
        }

        public static float Perlin1D (Vector3 point, float scale) {
            point *= scale;
            int x = Mathf.FloorToInt (point.x);
            float t = point.x - x;

            float g0 = gradients1D[hash[x & hashMask] & gradientsMask1D];
            float g1 = gradients1D[hash[(x + 1) & hashMask] & gradientsMask1D];

            float v0 = g0 * t;
            float v1 = g1 * (t - 1f);
            return Mathf.Lerp (v0, v1, Smooth (point.x - x));
        }

        public static float Perlin2D (Vector3 point, float scale) {
            point *= scale;
            int x = Mathf.FloorToInt (point.x);
            float tx = point.x - x;
            int y = Mathf.FloorToInt (point.y);
            float ty = point.y - y;

            int h0 = hash[x & hashMask];
            int h1 = hash[(x + 1) & hashMask];

            Vector2 g00 = gradients2D[hash[(h0 + y) & hashMask] & gradientsMask2D];
            Vector2 g01 = gradients2D[hash[(h0 + y + 1) & hashMask] & gradientsMask2D];
            Vector2 g10 = gradients2D[hash[(h1 + y) & hashMask] & gradientsMask2D];
            Vector2 g11 = gradients2D[hash[(h1 + y + 1) & hashMask] & gradientsMask2D];

            float v00 = Dot (g00, tx, ty);
            float v01 = Dot (g01, tx, ty - 1f);
            float v10 = Dot (g10, tx - 1f, ty);
            float v11 = Dot (g11, tx - 1f, ty - 1f);

            float vx0 = Mathf.Lerp (v00, v10, Smooth (tx));
            float vx1 = Mathf.Lerp (v01, v11, Smooth (tx));
            return Mathf.Lerp (vx0, vx1, Smooth (ty)) * sqr2;
        }

        public static float Perlin3D (Vector3 point, float scale) {
            point *= scale;
            int x = Mathf.FloorToInt (point.x);
            float tx = point.x - x;
            int y = Mathf.FloorToInt (point.y);
            float ty = point.y - y;
            int z = Mathf.FloorToInt (point.z);
            float tz = point.z - z;

            int h0 = hash[x & hashMask];
            int h1 = hash[(x + 1) & hashMask];
            int h00 = hash[(h0 + y) & hashMask];
            int h10 = hash[(h1 + y) & hashMask];
            int h01 = hash[(h0 + y + 1) & hashMask];
            int h11 = hash[(h1 + y + 1) & hashMask];

            Vector3 g000 = gradients3D[hash[(h00 + z) & hashMask] & gradientsMask3D];
            Vector3 g010 = gradients3D[hash[(h01 + z) & hashMask] & gradientsMask3D];
            Vector3 g100 = gradients3D[hash[(h10 + z) & hashMask] & gradientsMask3D];
            Vector3 g110 = gradients3D[hash[(h11 + z) & hashMask] & gradientsMask3D];
            Vector3 g001 = gradients3D[hash[(h00 + z + 1) & hashMask] & gradientsMask3D];
            Vector3 g011 = gradients3D[hash[(h01 + z + 1) & hashMask] & gradientsMask3D];
            Vector3 g101 = gradients3D[hash[(h10 + z + 1) & hashMask] & gradientsMask3D];
            Vector3 g111 = gradients3D[hash[(h11 + z + 1) & hashMask] & gradientsMask3D];

            float v000 = Dot (g000, tx, ty, tz);
            float v010 = Dot (g010, tx, ty - 1f, tz);
            float v100 = Dot (g100, tx - 1f, ty, tz);
            float v110 = Dot (g110, tx - 1f, ty - 1f, tz);
            float v001 = Dot (g001, tx, ty, tz - 1f);
            float v011 = Dot (g011, tx, ty - 1f, tz - 1f);
            float v101 = Dot (g101, tx - 1f, ty, tz - 1f);
            float v111 = Dot (g111, tx - 1f, ty - 1f, tz - 1f);

            float vx00 = Mathf.Lerp (v000, v100, Smooth (tx));
            float vx10 = Mathf.Lerp (v010, v110, Smooth (tx));
            float vx01 = Mathf.Lerp (v001, v101, Smooth (tx));
            float vx11 = Mathf.Lerp (v011, v111, Smooth (tx));
            float vxy0 = Mathf.Lerp (vx00, vx10, Smooth (ty));
            float vxy1 = Mathf.Lerp (vx01, vx11, Smooth (ty));
            return Mathf.Lerp (vxy0, vxy1, Smooth (point.z - z));
        }

        public static float Octaves (NoiseMethod method, Vector3 point, float scale, int octaves, float lacunarity, float persistence) {
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