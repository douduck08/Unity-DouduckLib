using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public class MathUtil {

        public static readonly float pi = Mathf.PI;
        public static readonly float halfPi = Mathf.PI / 2.0f;
        public static readonly float thirdPi = Mathf.PI / 3.0f;
        public static readonly float radToDeg = 180.0f / Mathf.PI;
        public static readonly float degToRad = Mathf.PI / 180.0f;

        public float GetDirectionAngle (Vector2 vector) {
            if (vector.y >= 0) {
                return Vector2.Angle (Vector2.right, vector);
            } else {
                return 360f - Vector2.Angle (Vector2.right, vector);
            }
        }

        public static float FastPow (float num, int exp) {
            float result = 1f;
            while (exp > 0) {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }
            return result;
        }
    }
}