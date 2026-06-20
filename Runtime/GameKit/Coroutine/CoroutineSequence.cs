using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public struct CoroutineSequenceYieldInstruction : IEnumerator
    {
        readonly Coroutine _coroutine;
        bool _isDone;

        public bool IsValid => _coroutine != null;

        public CoroutineSequenceYieldInstruction(Coroutine coroutine)
        {
            _coroutine = coroutine;
            _isDone = false;
        }

        public object Current => _coroutine;

        public bool MoveNext()
        {
            if (!_isDone)
            {
                _isDone = true;
                return _coroutine != null;
            }
            return false;
        }

        public void Reset() {}
    }
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

            public IEnumerator GetEnumerator(MonoBehaviour owner, List<Coroutine> coroutines)
            {
                if (_jointEnumerators.Count == 0)
                {
                    yield return InternalEnumerator;
                }
                else
                {
                    var all = new List<IEnumerator>(_jointEnumerators.Count + 1);
                    all.Add(InternalEnumerator);
                    all.AddRange(_jointEnumerators);

                    var wait = new WaitForCounter(all.Count);

                    for (int i = 0; i < all.Count; i++)
                    {
                        var coroutine = owner.StartCoroutine(CoroutineHandle.CallbackEnumerator(all[i], wait.Decrement));
                        coroutines.Add(coroutine);
                    }
                    yield return wait;
                }
            }

            public void AddJointEnumerator(IEnumerator enumerator)
            {
                _jointEnumerators.Add(enumerator);
            }
        }

        List<Coroutine> _coroutines = new();
        List<InsertedEnumerator> _insertedEnumerators = new();
        List<AppendedEnumerator> _appendedEnumerators = new();
        List<CoroutineSequence> _sequences = new();

        MonoBehaviour _owner;
        Action _onComplete;
        bool _isStarted;

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
            if (_appendedEnumerators.Count == 0)
            {
                throw new InvalidOperationException("Cannot call Joint before any Append has been added.");
            }
            var index = _appendedEnumerators.Count - 1;
            _appendedEnumerators[index].AddJointEnumerator(enumerator);
            return this;
        }

        public CoroutineSequence Joint(Action callback)
        {
            if (_appendedEnumerators.Count == 0)
            {
                throw new InvalidOperationException("Cannot call Joint before any Append has been added.");
            }
            var index = _appendedEnumerators.Count - 1;
            _appendedEnumerators[index].AddJointEnumerator(GetCallbackEnumerator(callback));
            return this;
        }

        public CoroutineSequence Joint(CoroutineSequence coroutineSequence)
        {
            if (_appendedEnumerators.Count == 0)
            {
                throw new InvalidOperationException("Cannot call Joint before any Append has been added.");
            }
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

        public CoroutineSequenceYieldInstruction StartCoroutine()
        {
            if (_isStarted)
            {
                Debug.LogWarning("[CoroutineSequence] Sequence has already been started. A CoroutineSequence can only be started once.");
                return new CoroutineSequenceYieldInstruction(null);
            }
            _isStarted = true;

            if (_owner == null)
            {
                Debug.LogError("[CoroutineSequence] Cannot start sequence because the owner MonoBehaviour is null.");
                return new CoroutineSequenceYieldInstruction(null);
            }

            var coroutine = _owner.StartCoroutine(GetEnumerator());
            _coroutines.Add(coroutine);
            return new CoroutineSequenceYieldInstruction(coroutine);
        }

        public void StopCoroutine()
        {
            if (_owner != null)
            {
                for (int i = 0; i < _coroutines.Count; i++)
                {
                    _owner.StopCoroutine(_coroutines[i]);
                }
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
            yield return CoroutineUtil.GetWaitForSeconds(seconds);
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
                yield return _appendedEnumerators[i].GetEnumerator(_owner, _coroutines);
            }

            _onComplete?.Invoke();
        }
    }
}
