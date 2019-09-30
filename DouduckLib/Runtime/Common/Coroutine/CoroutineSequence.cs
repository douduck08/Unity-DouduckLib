using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib {
    public class CoroutineSequence {

        class InsertedEnumerator {
            public IEnumerator InternalEnumerator { get; private set; }
            float atPosition;

            public InsertedEnumerator (float atPosition, IEnumerator enumerator) {
                this.atPosition = atPosition;
                InternalEnumerator = enumerator;
            }

            public IEnumerator GetEnumerator () {
                var waitTime = Time.time + atPosition;
                while (Time.time < waitTime) {
                    yield return null;
                }

                yield return InternalEnumerator;
            }
        }

        class AppendedEnumerator {
            public IEnumerator InternalEnumerator { get; private set; }
            List<IEnumerator> jointEnumerators = new List<IEnumerator> ();

            public AppendedEnumerator (IEnumerator enumerator) {
                InternalEnumerator = enumerator;
            }

            public IEnumerator GetEnumerator () {
                yield return InternalEnumerator;
            }

            public void AddJointEnumerator (IEnumerator enumerator) {
                jointEnumerators.Add (enumerator);
            }

            public void StartJointCoroutine (MonoBehaviour owner, List<Coroutine> coroutines) {
                for (int i = 0; i < jointEnumerators.Count; i++) {
                    coroutines.Add (owner.StartCoroutine (jointEnumerators[i]));
                }
            }
        }

        List<Coroutine> coroutines = new List<Coroutine> ();
        List<InsertedEnumerator> insertedEnumerators = new List<InsertedEnumerator> ();
        List<AppendedEnumerator> appendedEnumerators = new List<AppendedEnumerator> ();
        List<CoroutineSequence> sequences = new List<CoroutineSequence> ();

        MonoBehaviour owner;
        Action onComplete;

        public CoroutineSequence (MonoBehaviour owner) {
            this.owner = owner;
        }

        public CoroutineSequence Insert (float atPosition, IEnumerator enumerator) {
            insertedEnumerators.Add (new InsertedEnumerator (atPosition, enumerator));
            return this;
        }

        public CoroutineSequence Insert (float atPosition, Action callback) {
            insertedEnumerators.Add (new InsertedEnumerator (atPosition, GetCallbackEnumerator (callback)));
            return this;
        }

        public CoroutineSequence Insert (float atPosition, CoroutineSequence coroutineSequence) {
            insertedEnumerators.Add (new InsertedEnumerator (atPosition, coroutineSequence.GetEnumerator ()));
            sequences.Add (coroutineSequence);
            return this;
        }

        public CoroutineSequence Append (IEnumerator enumerator) {
            appendedEnumerators.Add (new AppendedEnumerator (enumerator));
            return this;
        }

        public CoroutineSequence Append (Action callback) {
            appendedEnumerators.Add (new AppendedEnumerator (GetCallbackEnumerator (callback)));
            return this;
        }

        public CoroutineSequence Append (CoroutineSequence coroutineSequence) {
            appendedEnumerators.Add (new AppendedEnumerator (coroutineSequence.GetEnumerator ()));
            sequences.Add (coroutineSequence);
            return this;
        }

        public CoroutineSequence AppendInterval (float seconds) {
            appendedEnumerators.Add (new AppendedEnumerator (GetWaitForSecondsEnumerator (seconds)));
            return this;
        }

        public CoroutineSequence Joint (IEnumerator enumerator) {
            var index = appendedEnumerators.Count - 1;
            appendedEnumerators[index].AddJointEnumerator (enumerator);
            return this;
        }

        public CoroutineSequence Joint (Action callback) {
            var index = appendedEnumerators.Count - 1;
            appendedEnumerators[index].AddJointEnumerator (GetCallbackEnumerator (callback));
            return this;
        }

        public CoroutineSequence Joint (CoroutineSequence coroutineSequence) {
            var index = appendedEnumerators.Count - 1;
            appendedEnumerators[index].AddJointEnumerator (coroutineSequence.GetEnumerator ());
            sequences.Add (coroutineSequence);
            return this;
        }

        public CoroutineSequence OnComplete (Action callback) {
            onComplete += callback;
            return this;

        }

        public Coroutine StartCoroutine () {
            var coroutine = owner.StartCoroutine (GetEnumerator ());
            coroutines.Add (coroutine);
            return coroutine;
        }

        public void StopCoroutine () {
            for (int i = 0; i < coroutines.Count; i++) {
                owner.StopCoroutine (coroutines[i]);
            }
            for (int i = 0; i < insertedEnumerators.Count; i++) {
                owner.StopCoroutine (insertedEnumerators[i].InternalEnumerator);
            }
            for (int i = 0; i < appendedEnumerators.Count; i++) {
                owner.StopCoroutine (appendedEnumerators[i].InternalEnumerator);
            }
            for (int i = 0; i < sequences.Count; i++) {
                sequences[i].StopCoroutine ();
            }
            coroutines.Clear ();
            insertedEnumerators.Clear ();
            appendedEnumerators.Clear ();
            sequences.Clear ();
        }

        IEnumerator GetCallbackEnumerator (Action callback) {
            callback ();
            yield break;
        }

        IEnumerator GetWaitForSecondsEnumerator (float seconds) {
            yield return new WaitForSeconds (seconds);
        }

        IEnumerator GetEnumerator () {
            for (int i = 0; i < insertedEnumerators.Count; i++) {
                var coroutine = owner.StartCoroutine (insertedEnumerators[i].GetEnumerator ());
                coroutines.Add (coroutine);
            }

            for (int i = 0; i < appendedEnumerators.Count; i++) {
                appendedEnumerators[i].StartJointCoroutine (owner, coroutines);
                yield return appendedEnumerators[i].GetEnumerator ();
            }

            if (onComplete != null) {
                onComplete.Invoke ();
            }
        }
    }
}