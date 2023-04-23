using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class CoroutineUtil : SingletonComponentAuto<CoroutineUtil>
    {
        protected internal override void OnSingletonAwake()
        {
            MarkAsCrossSceneSingleton();
        }

        public static void StartCoroutineOnDontDestroy(IEnumerator routine)
        {
            instance.StartCoroutine(routine);
        }

        public static void RunDelaySeconds(Action callback, float seconds)
        {
            if (callback != null)
            {
                instance.StartCoroutine(DelaySeconds(callback, seconds));
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
                instance.StartCoroutine(DelaySeconds(callback, frames));
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
