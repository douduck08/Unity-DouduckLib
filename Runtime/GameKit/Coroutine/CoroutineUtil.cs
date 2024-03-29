﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class CoroutineUtil : GlobalSingletonComponent<CoroutineUtil>
    {
        public static void StartCoroutineOnDontDestroy(IEnumerator routine)
        {
            Get()?.StartCoroutine(routine);
        }

        public static void RunDelaySeconds(Action callback, float seconds)
        {
            if (callback != null)
            {
                Get()?.StartCoroutine(DelaySeconds(callback, seconds));
            }
        }

        static IEnumerator DelaySeconds(Action callback, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            callback.Invoke();
        }

        public static void RunDelayFrames(Action callback, int frames)
        {
            if (callback != null)
            {
                Get()?.StartCoroutine(DelayFrames(callback, frames));
            }
        }

        static IEnumerator DelayFrames(Action callback, int frames)
        {
            while (frames > 0)
            {
                frames -= 1;
                yield return null;
            }
            callback.Invoke();
        }
    }
}
