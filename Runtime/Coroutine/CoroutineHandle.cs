using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public static class CoroutineHandle
    {
        public static IEnumerator Delay(float seconds, Action onFinish)
        {
            var wait = new WaitForSeconds(seconds);

            yield return wait;

            if (onFinish != null)
            {
                onFinish.Invoke();
            }
        }

        public static IEnumerator DelayEnumerator(float seconds, IEnumerator enumerator, Action onFinish)
        {
            var wait = new WaitForSeconds(seconds);

            yield return wait;
            yield return enumerator;

            if (onFinish != null)
            {
                onFinish.Invoke();
            }
        }

        public static IEnumerator CallbackEnumerator(IEnumerator enumerator, Action onFinish)
        {
            yield return enumerator;

            if (onFinish != null)
            {
                onFinish.Invoke();
            }
        }

        public static IEnumerator ParallelEnumerator(MonoBehaviour owner, Action onFinish, params IEnumerator[] enumerators)
        {
            var counter = enumerators.Length;
            var wait = new WaitUntil(() => counter == 0);

            for (int i = 0; i < enumerators.Length; i++)
            {
                owner.StartCoroutine(CallbackEnumerator(enumerators[i], () => counter--));
            }
            yield return wait;

            if (onFinish != null)
            {
                onFinish.Invoke();
            }
        }

        public static IEnumerator QueueEnumerator(Action onFinish, params IEnumerator[] enumerators)
        {
            for (int i = 0; i < enumerators.Length; i++)
            {
                yield return enumerators[i];
            }

            if (onFinish != null)
            {
                onFinish.Invoke();
            }
        }
    }
}
