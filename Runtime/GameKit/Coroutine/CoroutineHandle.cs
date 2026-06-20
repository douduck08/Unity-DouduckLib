using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    internal class WaitForCounter : CustomYieldInstruction
    {
        int _counter;

        public WaitForCounter(int count)
        {
            _counter = count;
        }

        public void Decrement()
        {
            _counter--;
        }

        public override bool keepWaiting => _counter > 0;
    }

    public static class CoroutineHandle
    {
        public static IEnumerator Delay(float seconds, Action onFinish)
        {
            yield return CoroutineUtil.GetWaitForSeconds(seconds);

            onFinish?.Invoke();
        }

        public static IEnumerator DelayEnumerator(float seconds, IEnumerator enumerator, Action onFinish)
        {
            yield return CoroutineUtil.GetWaitForSeconds(seconds);
            yield return enumerator;

            onFinish?.Invoke();
        }

        public static IEnumerator CallbackEnumerator(IEnumerator enumerator, Action onFinish)
        {
            yield return enumerator;

            onFinish?.Invoke();
        }

        public static IEnumerator ParallelEnumerator(MonoBehaviour owner, Action onFinish, params IEnumerator[] enumerators)
        {
            if (enumerators == null || enumerators.Length == 0)
            {
                onFinish?.Invoke();
                yield break;
            }

            var wait = new WaitForCounter(enumerators.Length);

            for (int i = 0; i < enumerators.Length; i++)
            {
                owner.StartCoroutine(CallbackEnumerator(enumerators[i], wait.Decrement));
            }
            yield return wait;

            onFinish?.Invoke();
        }

        public static IEnumerator QueueEnumerator(Action onFinish, params IEnumerator[] enumerators)
        {
            if (enumerators != null)
            {
                for (int i = 0; i < enumerators.Length; i++)
                {
                    yield return enumerators[i];
                }
            }

            onFinish?.Invoke();
        }
    }
}
