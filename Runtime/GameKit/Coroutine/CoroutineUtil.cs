using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class CoroutineUtil : GlobalSingletonComponent<CoroutineUtil>
    {
        static readonly Dictionary<float, WaitForSeconds> _waitForSecondsCache = new();

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (!_waitForSecondsCache.TryGetValue(seconds, out var wait))
            {
                wait = new WaitForSeconds(seconds);
                _waitForSecondsCache.Add(seconds, wait);
            }
            return wait;
        }

        public static void StartCoroutineOnDontDestroy(IEnumerator routine)
        {
            var util = Get();
            if (util != null)
            {
                util.StartCoroutine(routine);
            }
        }

        public static void RunDelaySeconds(float seconds, Action callback)
        {
            if (callback != null)
            {
                var util = Get();
                if (util != null)
                {
                    util.StartCoroutine(DelaySeconds(seconds, callback));
                }
            }
        }

        static IEnumerator DelaySeconds(float seconds, Action callback)
        {
            yield return GetWaitForSeconds(seconds);
            callback.Invoke();
        }

        public static void RunDelayFrames(int frames, Action callback)
        {
            if (callback != null)
            {
                var util = Get();
                if (util != null)
                {
                    util.StartCoroutine(DelayFrames(frames, callback));
                }
            }
        }

        static IEnumerator DelayFrames(int frames, Action callback)
        {
            if (frames <= 0)
            {
                yield return null;
            }
            else
            {
                while (frames > 0)
                {
                    frames -= 1;
                    yield return null;
                }
            }
            callback.Invoke();
        }
    }
}
