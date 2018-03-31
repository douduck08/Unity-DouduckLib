using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
	public partial class Easing {
	    /* Standard Equation for Easing */
        public static float Linear (float t) { return t; }

        public static float EaseInQuad (float t) { return t * t; }
        public static float EaseOutQuad (float t) { return (2f - t) * t; }
        public static float EaseInOutQuad (float t) { return t < 0.5f ? EaseInQuad (t * 2f) * 0.5f : EaseOutQuad ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInQuad (float t) { return t < 0.5f ? EaseOutQuad (t * 2f) * 0.5f : EaseInQuad ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInCubic (float t) { return t * t * t; }
        public static float EaseOutCubic (float t) { return --t * t * t + 1f; }
        public static float EaseInOutCubic (float t) { return t < 0.5f ? EaseInCubic (t * 2f) * 0.5f : EaseOutCubic ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInCubic (float t) { return t < 0.5f ? EaseOutCubic (t * 2f) * 0.5f : EaseInCubic ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInQuart (float t) { return (t *= t) * t; }
        public static float EaseOutQuart (float t) {
            t -= 1f;
            return 1f - (t *= t) * t;
        }
        public static float EaseInOutQuart (float t) { return t < 0.5f ? EaseInQuart (t * 2f) * 0.5f : EaseOutQuart ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInQuart (float t) { return t < 0.5f ? EaseOutQuart (t * 2f) * 0.5f : EaseInQuart ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInQuint (float t) {
            float t2 = t * t;
            return t2 * t2 * t;
        }
        public static float EaseOutQuint (float t) {
            t -= 1f;
            float t2 = t * t;
            return t2 * t2 * t + 1f;
        }
        public static float EaseInOutQuint (float t) { return t < 0.5f ? EaseInQuint (t * 2f) * 0.5f : EaseOutQuint ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInQuint (float t) { return t < 0.5f ? EaseOutQuint (t * 2f) * 0.5f : EaseInQuint ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInSine (float t) { return 1f - Mathf.Cos (MathUtil.halfPi * t); }
        public static float EaseOutSine (float t) { return Mathf.Sin (MathUtil.halfPi * t); }
        public static float EaseInOutSine (float t) { return 0.5f - 0.5f * Mathf.Cos (t * Mathf.PI); }
        public static float EaseOutInSine (float t) { return t < 0.5f ? EaseOutSine (t * 2f) * 0.5f : EaseInSine ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInExpo (float t) { return Mathf.Pow (2f, 10f * t - 10f); }
        public static float EaseOutExpo (float t) { return 1f - Mathf.Pow (2f, -10f * t); }
        public static float EaseInOutExpo (float t) { return t < 0.5f ? EaseInExpo (t * 2f) * 0.5f : EaseOutExpo ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInExpo (float t) { return t < 0.5f ? EaseOutExpo (t * 2f) * 0.5f : EaseInExpo ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInCirc (float t) { return 1f - Mathf.Sqrt (1f - t * t); }
        public static float EaseOutCirc (float t) { return Mathf.Sqrt (1f - --t * t); }
        public static float EaseInOutCirc (float t) { return t < 0.5f ? EaseInCirc (t * 2f) * 0.5f : EaseOutCirc ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInCirc (float t) { return t < 0.5f ? EaseOutCirc (t * 2f) * 0.5f : EaseInCirc ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInBack (float t) { return t * t * (2.70158f * t - 1.70158f); }
        public static float EaseOutBack (float t) { return 1f + --t * t * (2.70158f * t + 1.70158f); }
        public static float EaseInOutBack (float t) { return t < 0.5f ? EaseInBack (t * 2f) * 0.5f : EaseOutBack ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInBack (float t) { return t < 0.5f ? EaseOutBack (t * 2f) * 0.5f : EaseInBack ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInElastic (float t) { return Mathf.Pow (2f, 10f * t - 10f) * Mathf.Sin (t * Mathf.PI * 4.5f); }
        public static float EaseOutElastic (float t) {
            t = 1f - t;
            return 1f - Mathf.Pow (2f, 10f * t - 10f) * Mathf.Sin (t * Mathf.PI * 4.5f);
        }
        public static float EaseInOutElastic (float t) { return t < 0.5f ? EaseInElastic (t * 2f) * 0.5f : EaseOutElastic ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInElastic (float t) { return t < 0.5f ? EaseOutElastic (t * 2f) * 0.5f : EaseInElastic ((t * 2f) - 1f) * 0.5f + 0.5f; }

        public static float EaseInBounce (float t) { return 1f - EaseOutBounce (1f - t); }
        public static float EaseOutBounce (float t) {
            if (t < 1 / 2.75f) {
                return 7.5625f * t * t;
            } else if (t < 2 / 2.75f) {
                return 7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f;
            } else if (t < 2.5 / 2.75f) {
                return 7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f;
            } else {
                return 7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f;
            }
        }
        public static float EaseInOutBounce (float t) { return t < 0.5f ? EaseInBounce (t * 2f) * 0.5f : EaseOutBounce ((t * 2f) - 1f) * 0.5f + 0.5f; }
        public static float EaseOutInBounce (float t) { return t < 0.5f ? EaseOutBounce (t * 2f) * 0.5f : EaseInBounce ((t * 2f) - 1f) * 0.5f + 0.5f; }
	}
}