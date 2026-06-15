using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class CoroutineSequence
    {

        class InsertedEnumerator
        {
            public IEnumerator InternalEnumerator { get; private set; }
            float _atPosition;

            public InsertedEnumerator(float atPosition, IEnumerator enumerator)
            {
                _atPosition = atPosition;
                InternalEnumerator = enumerator;
            }

            public IEnumerator GetEnumerator()
            {
                var waitTime = Time.time + _atPosition;
                while (Time.time < waitTime)
                {
                    yield return null;
                }

                yield return InternalEnumerator;
            }
        }

        class AppendedEnumerator
        {
            public IEnumerator InternalEnumerator { get; private set; }
            List<IEnumerator> _jointEnumerators = new List<IEnumerator>();

            public AppendedEnumerator(IEnumerator enumerator)
            {
                InternalEnumerator = enumerator;
            }

            public IEnumerator GetEnumerator()
            {
                yield return InternalEnumerator;
            }

            public void AddJointEnumerator(IEnumerator enumerator)
            {
                _jointEnumerators.Add(enumerator);
            }

            public void StartJointCoroutine(MonoBehaviour owner, List<Coroutine> coroutines)
            {
                for (int i = 0; i < _jointEnumerators.Count; i++)
                {
                    coroutines.Add(owner.StartCoroutine(_jointEnumerators[i]));
                }
            }
        }

        List<Coroutine> _coroutines = new List<Coroutine>();
        List<InsertedEnumerator> _insertedEnumerators = new List<InsertedEnumerator>();
        List<AppendedEnumerator> _appendedEnumerators = new List<AppendedEnumerator>();
        List<CoroutineSequence> _sequences = new List<CoroutineSequence>();

        MonoBehaviour _owner;
        Action _onComplete;

        public CoroutineSequence(MonoBehaviour owner)
        {
            _owner = owner;
        }

        public CoroutineSequence Insert(float atPosition, IEnumerator enumerator)
        {
            _insertedEnumerators.Add(new InsertedEnumerator(atPosition, enumerator));
            return this;
        }

        public CoroutineSequence Insert(float atPosition, Action callback)
        {
            _insertedEnumerators.Add(new InsertedEnumerator(atPosition, GetCallbackEnumerator(callback)));
            return this;
        }

        public CoroutineSequence Insert(float atPosition, CoroutineSequence coroutineSequence)
        {
            _insertedEnumerators.Add(new InsertedEnumerator(atPosition, coroutineSequence.GetEnumerator()));
            _sequences.Add(coroutineSequence);
            return this;
        }

        public CoroutineSequence Append(IEnumerator enumerator)
        {
            _appendedEnumerators.Add(new AppendedEnumerator(enumerator));
            return this;
        }

        public CoroutineSequence Append(Action callback)
        {
            _appendedEnumerators.Add(new AppendedEnumerator(GetCallbackEnumerator(callback)));
            return this;
        }

        public CoroutineSequence Append(CoroutineSequence coroutineSequence)
        {
            _appendedEnumerators.Add(new AppendedEnumerator(coroutineSequence.GetEnumerator()));
            _sequences.Add(coroutineSequence);
            return this;
        }

        public CoroutineSequence AppendInterval(float seconds)
        {
            _appendedEnumerators.Add(new AppendedEnumerator(GetWaitForSecondsEnumerator(seconds)));
            return this;
        }

        public CoroutineSequence Joint(IEnumerator enumerator)
        {
            var index = _appendedEnumerators.Count - 1;
            _appendedEnumerators[index].AddJointEnumerator(enumerator);
            return this;
        }

        public CoroutineSequence Joint(Action callback)
        {
            var index = _appendedEnumerators.Count - 1;
            _appendedEnumerators[index].AddJointEnumerator(GetCallbackEnumerator(callback));
            return this;
        }

        public CoroutineSequence Joint(CoroutineSequence coroutineSequence)
        {
            var index = _appendedEnumerators.Count - 1;
            _appendedEnumerators[index].AddJointEnumerator(coroutineSequence.GetEnumerator());
            _sequences.Add(coroutineSequence);
            return this;
        }

        public CoroutineSequence OnComplete(Action callback)
        {
            _onComplete += callback;
            return this;
        }

        public Coroutine StartCoroutine()
        {
            var coroutine = _owner.StartCoroutine(GetEnumerator());
            _coroutines.Add(coroutine);
            return coroutine;
        }

        public void StopCoroutine()
        {
            for (int i = 0; i < _coroutines.Count; i++)
            {
                _owner.StopCoroutine(_coroutines[i]);
            }
            for (int i = 0; i < _insertedEnumerators.Count; i++)
            {
                _owner.StopCoroutine(_insertedEnumerators[i].InternalEnumerator);
            }
            for (int i = 0; i < _appendedEnumerators.Count; i++)
            {
                _owner.StopCoroutine(_appendedEnumerators[i].InternalEnumerator);
            }
            for (int i = 0; i < _sequences.Count; i++)
            {
                _sequences[i].StopCoroutine();
            }
            _coroutines.Clear();
            _insertedEnumerators.Clear();
            _appendedEnumerators.Clear();
            _sequences.Clear();
        }

        IEnumerator GetCallbackEnumerator(Action callback)
        {
            callback();
            yield break;
        }

        IEnumerator GetWaitForSecondsEnumerator(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _insertedEnumerators.Count; i++)
            {
                var coroutine = _owner.StartCoroutine(_insertedEnumerators[i].GetEnumerator());
                _coroutines.Add(coroutine);
            }

            for (int i = 0; i < _appendedEnumerators.Count; i++)
            {
                _appendedEnumerators[i].StartJointCoroutine(_owner, _coroutines);
                yield return _appendedEnumerators[i].GetEnumerator();
            }

            if (_onComplete != null)
            {
                _onComplete.Invoke();
            }
        }
    }
}
