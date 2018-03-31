using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DouduckLib {
    public class TweenHelper {

        public event Action onStart;
        public event Action onComplete;

        float m_delay = -1f;
        float m_durability;
        Func<float, float> m_easingFunc;
        Action<float> m_process;

        public static TweenHelper Create (float durability, Func<float, float> easingFunc, Action<float> process) {
            return new TweenHelper (durability, easingFunc, process);
        }

        public static TweenHelper Create (float durability, Action<float> process) {
            return new TweenHelper (durability, (t) => t, process);
        }

        private TweenHelper (float durability, Func<float, float> easingFunc, Action<float> process) {
            m_durability = durability;
            m_easingFunc = easingFunc;
            m_process = process;
        }

        public TweenHelper SetDelay (float second) {
            m_delay = second;
            return this;
        }

        public TweenHelper OnStart (Action callback) {
            onStart += callback;
            return this;
        }

        public TweenHelper OnComplete (Action callback) {
            onComplete += callback;
            return this;
        }

        public void Run () {
            CoroutineUtil.Run (DoTweening ());
        }

        IEnumerator<float> DoTweening () {
            float timer = 0f;
            while (timer < m_delay) {
                yield return 0;
                timer += Time.deltaTime;
            }

            if (onStart != null) {
                onStart.Invoke ();
            }

            timer = 0f;
            while (timer <= m_durability) {
                m_process.Invoke (m_easingFunc (timer / m_durability));
                yield return 0;
                timer += Time.deltaTime;
            }

            if (onComplete != null) {
                onComplete.Invoke ();
            }
        }
    }
}