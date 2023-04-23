using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class Noise
    {
        static int[] hash = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
        static int hashMask = 255;

        static int Hash(float x)
        {
            return hash[Mathf.FloorToInt(x) & hashMask];
        }

        static int Hash(float x, float y)
        {
            return Hash(Hash(x) + y);
        }

        static int Hash(float x, float y, float z)
        {
            return Hash(Hash(Hash(x) + y) + z);
        }

        static float Smooth(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        public static float Value1D(float x, float scale = 1f)
        {
            x *= scale;
            int h0 = Hash(x);
            int h1 = Hash(x + 1f);
            return Mathf.Lerp(h0, h1, Smooth(x % 1f)) * (1f / hashMask);
        }

        public static float Value2D(Vector2 point, float scale = 1f)
        {
            point *= scale;
            int h00 = Hash(point.x, point.y);
            int h10 = Hash(point.x + 1f, point.y);
            int h01 = Hash(point.x, point.y + 1f);
            int h11 = Hash(point.x + 1f, point.y + 1f);
            float hx0 = Mathf.Lerp(h00, h10, Smooth(point.x % 1f));
            float hx1 = Mathf.Lerp(h01, h11, Smooth(point.x % 1f));
            return Mathf.Lerp(hx0, hx1, Smooth(point.y % 1f)) * (1f / hashMask);
        }

        public static float Value3D(Vector3 point, float scale)
        {
            point *= scale;
            int h000 = Hash(point.x, point.y, point.z);
            int h100 = Hash(point.x + 1f, point.y, point.z);
            int h010 = Hash(point.x, point.y + 1f, point.z);
            int h110 = Hash(point.x + 1f, point.y + 1f, point.z);
            int h001 = Hash(point.x, point.y, point.z + 1f);
            int h101 = Hash(point.x + 1f, point.y, point.z + 1f);
            int h011 = Hash(point.x, point.y + 1f, point.z + 1f);
            int h111 = Hash(point.x + 1f, point.y + 1f, point.z + 1f);
            float hx00 = Mathf.Lerp(h000, h100, Smooth(point.x % 1f));
            float hx10 = Mathf.Lerp(h010, h110, Smooth(point.x % 1f));
            float hx01 = Mathf.Lerp(h001, h101, Smooth(point.x % 1f));
            float hx11 = Mathf.Lerp(h011, h111, Smooth(point.x % 1f));
            float hxy0 = Mathf.Lerp(hx00, hx10, Smooth(point.y % 1f));
            float hxy1 = Mathf.Lerp(hx01, hx11, Smooth(point.y % 1f));
            return Mathf.Lerp(hxy0, hxy1, Smooth(point.z % 1f)) * (1f / hashMask);
            ;
        }

        static float[] gradients1D = { 1f, -1f };
        static int gradientsMask1D = 1;

        public static float Perlin1D(float x, float scale)
        {
            x *= scale;
            float g0 = gradients1D[Hash(x) & gradientsMask1D];
            float g1 = gradients1D[Hash(x + 1f) & gradientsMask1D];

            float t = x % 1f;
            float v0 = g0 * t;
            float v1 = g1 * (t - 1f);

            return Mathf.Lerp(v0, v1, Smooth(t)) * 0.5f + 0.5f;
        }

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

        static float Dot(Vector2 g, float x, float y)
        {
            return g.x * x + g.y * y;
        }

        public static float Perlin2D(Vector2 point, float scale)
        {
            point *= scale;
            Vector2 g00 = gradients2D[Hash(point.x, point.y) & gradientsMask2D];
            Vector2 g01 = gradients2D[Hash(point.x, point.y + 1f) & gradientsMask2D];
            Vector2 g10 = gradients2D[Hash(point.x + 1f, point.y) & gradientsMask2D];
            Vector2 g11 = gradients2D[Hash(point.x + 1f, point.y + 1f) & gradientsMask2D];

            float tx = point.x % 1f;
            float ty = point.y % 1f;
            float v00 = Dot(g00, tx, ty);
            float v01 = Dot(g01, tx, ty - 1f);
            float v10 = Dot(g10, tx - 1f, ty);
            float v11 = Dot(g11, tx - 1f, ty - 1f);

            float vx0 = Mathf.Lerp(v00, v10, Smooth(tx));
            float vx1 = Mathf.Lerp(v01, v11, Smooth(tx));
            return Mathf.Lerp(vx0, vx1, Smooth(ty)) * MathUtil.sqrt2 * 0.5f + 0.5f;
        }

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

        static float Dot(Vector3 g, float x, float y, float z)
        {
            return g.x * x + g.y * y + g.z * z;
        }

        public static float Perlin3D(Vector3 point, float scale)
        {
            point *= scale;
            Vector3 g000 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];
            Vector3 g010 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];
            Vector3 g100 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];
            Vector3 g110 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];
            Vector3 g001 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];
            Vector3 g011 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];
            Vector3 g101 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];
            Vector3 g111 = gradients3D[Hash(point.x, point.y, point.z) & gradientsMask3D];

            float tx = point.x % 1f;
            float ty = point.y % 1f;
            float tz = point.z % 1f;
            float v000 = Dot(g000, tx, ty, tz);
            float v010 = Dot(g010, tx, ty - 1f, tz);
            float v100 = Dot(g100, tx - 1f, ty, tz);
            float v110 = Dot(g110, tx - 1f, ty - 1f, tz);
            float v001 = Dot(g001, tx, ty, tz - 1f);
            float v011 = Dot(g011, tx, ty - 1f, tz - 1f);
            float v101 = Dot(g101, tx - 1f, ty, tz - 1f);
            float v111 = Dot(g111, tx - 1f, ty - 1f, tz - 1f);

            float vx00 = Mathf.Lerp(v000, v100, Smooth(tx));
            float vx10 = Mathf.Lerp(v010, v110, Smooth(tx));
            float vx01 = Mathf.Lerp(v001, v101, Smooth(tx));
            float vx11 = Mathf.Lerp(v011, v111, Smooth(tx));
            float vxy0 = Mathf.Lerp(vx00, vx10, Smooth(ty));
            float vxy1 = Mathf.Lerp(vx01, vx11, Smooth(ty));
            return Mathf.Lerp(vxy0, vxy1, Smooth(tz)) * 0.5f + 0.5f;
        }
    }
}
