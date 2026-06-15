using UnityEngine;
using System.Collections;

namespace DouduckLib
{
    public abstract class NumericSpring<T>
    {
        protected T _target;
        protected T _current;
        protected T _delta;

        protected float _dumpingRatio;
        protected float _frequency;
        protected float _sensitivity;

        public NumericSpring(T origin, float frequency, float dumpingRatio = 1.0f, float sensitivity = 0.01f)
        {
            _target = origin;
            _current = origin;
            _delta = default(T);

            _frequency = frequency;
            _dumpingRatio = dumpingRatio;
            _sensitivity = sensitivity;
        }

        public void SetTarget(T target)
        {
            _target = target;
        }

        public T Next(float deltaTime)
        {
            float ww = _frequency * _frequency;
            float wwt = ww * deltaTime;
            float wwtt = wwt * deltaTime;
            float f = 1.0f + 2.0f * deltaTime * _dumpingRatio * _frequency;
            float detInv = 1.0f / (f + wwtt);

            _current = UpdateCurrent(wwtt * detInv, f * detInv, deltaTime * detInv);
            _delta = UpdateDelta(wwt * detInv, -wwt * detInv, detInv);
            if (Insensitive())
            {
                _current = _target;
            }
            return _current;
        }

        protected abstract T UpdateCurrent(float targetVar, float currentVar, float deltaVar);
        protected abstract T UpdateDelta(float targetVar, float currentVar, float deltaVar);
        protected abstract bool Insensitive();
    }

    public class NumericSpring : NumericSpring<float>
    {
        public NumericSpring(float origin, float frequency, float dumpingRatio = 1, float sensitivity = 0.01F) : base(origin, frequency, dumpingRatio, sensitivity) { }

        protected override float UpdateCurrent(float targetVar, float currentVar, float deltaVar)
        {
            return targetVar * _target + currentVar * _current + deltaVar * _delta;
        }

        protected override float UpdateDelta(float targetVar, float currentVar, float deltaVar)
        {
            return targetVar * _target + currentVar * _current + deltaVar * _delta;
        }

        protected override bool Insensitive()
        {
            return Mathf.Abs(_current - _target) < _sensitivity;
        }
    }

    public class NumericSpring2 : NumericSpring<Vector2>
    {
        public NumericSpring2(Vector2 origin, float frequency, float dumpingRatio = 1, float sensitivity = 0.01F) : base(origin, frequency, dumpingRatio, sensitivity) { }

        protected override Vector2 UpdateCurrent(float targetVar, float currentVar, float deltaVar)
        {
            return targetVar * _target + currentVar * _current + deltaVar * _delta;
        }

        protected override Vector2 UpdateDelta(float targetVar, float currentVar, float deltaVar)
        {
            return targetVar * _target + currentVar * _current + deltaVar * _delta;
        }

        protected override bool Insensitive()
        {
            return Vector2.Distance(_current, _target) < _sensitivity;
        }
    }

    public class NumericSpring3 : NumericSpring<Vector3>
    {
        public NumericSpring3(Vector3 origin, float frequency, float dumpingRatio = 1, float sensitivity = 0.01F) : base(origin, frequency, dumpingRatio, sensitivity) { }

        protected override Vector3 UpdateCurrent(float targetVar, float currentVar, float deltaVar)
        {
            return targetVar * _target + currentVar * _current + deltaVar * _delta;
        }

        protected override Vector3 UpdateDelta(float targetVar, float currentVar, float deltaVar)
        {
            return targetVar * _target + currentVar * _current + deltaVar * _delta;
        }

        protected override bool Insensitive()
        {
            return Vector3.Distance(_current, _target) < _sensitivity;
        }
    }
}
