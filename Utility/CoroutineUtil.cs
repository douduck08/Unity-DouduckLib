using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

namespace DouduckGame.Util {
    public class CoroutineUtil {
        public static void Delay (float sec, Action callback) {
            if (null != callback) {
                Timing.RunCoroutine (WaitForSeconds (sec, callback));
            }
        }

        private static IEnumerator<float> WaitForSeconds (float sec, Action callback) {
            yield return Timing.WaitForSeconds (sec);
            callback();
        }

        public static void WaitNextUpdate (Action callback) {
            if (null != callback) {
                Timing.RunCoroutine (WaitForUpdate (callback));
            }
        }

        public static void WaitNextUpdate (Action callback, Segment segment) {
            if (null != callback) {
                Timing.RunCoroutine (WaitForUpdate (callback), segment);
            }
        }

        private static IEnumerator<float> WaitForUpdate (Action callback) {
            yield return 0f;
            callback ();
        }
    }
}
