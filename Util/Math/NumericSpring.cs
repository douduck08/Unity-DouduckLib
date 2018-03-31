using UnityEngine;
using System.Collections;

namespace DouduckLib {
	public abstract class NumericSpring<T> {
		protected T m_target;
		protected T m_current;
		protected T m_delta;

		protected float m_dumpingRatio;
		protected float m_frequency;
		protected float m_sensitivity;

		public NumericSpring (T origin, float frequency, float dumpingRatio = 1.0f, float sensitivity = 0.01f) {
			m_target = origin;
			m_current = origin;
			m_delta = default(T);

			m_frequency = frequency;
			m_dumpingRatio = dumpingRatio;
			m_sensitivity = sensitivity;
		}

		public void SetTarget (T target) {
			m_target = target;
		}

		public T Next(float deltaTime) {
			float ww = m_frequency * m_frequency;
			float wwt = ww * deltaTime;
			float wwtt = wwt * deltaTime;
			float f = 1.0f + 2.0f * deltaTime * m_dumpingRatio * m_frequency;
			float detInv = 1.0f / (f + wwtt);

			m_current = UpdateCurrent (wwtt * detInv, f * detInv, deltaTime * detInv);
			m_delta = UpdateDelta (wwt * detInv, -wwt * detInv, detInv);
			if (Insensitive ()) {
				m_current = m_target;
			}
			return m_current;
		}

		protected abstract T UpdateCurrent (float targetVar, float currentVar, float deltaVar);
		protected abstract T UpdateDelta (float targetVar, float currentVar, float deltaVar);
		protected abstract bool Insensitive ();
	}

    public class NumericSpring : NumericSpring<float> {
        public NumericSpring (float origin, float frequency, float dumpingRatio = 1, float sensitivity = 0.01F) : base(origin, frequency, dumpingRatio, sensitivity) {}
        
        protected override float UpdateCurrent(float targetVar, float currentVar, float deltaVar) {
            return targetVar * m_target + currentVar * m_current + deltaVar * m_delta;
        }

        protected override float UpdateDelta(float targetVar, float currentVar, float deltaVar) {
            return targetVar * m_target + currentVar * m_current + deltaVar * m_delta;
        }

		protected override bool Insensitive () {
			return Mathf.Abs(m_current - m_target) < m_sensitivity;
        }
    }

	public class NumericSpring2 : NumericSpring<Vector2> {
        public NumericSpring2 (Vector2 origin, float frequency, float dumpingRatio = 1, float sensitivity = 0.01F) : base(origin, frequency, dumpingRatio, sensitivity) {}
        
        protected override Vector2 UpdateCurrent(float targetVar, float currentVar, float deltaVar) {
            return targetVar * m_target + currentVar * m_current + deltaVar * m_delta;
        }

        protected override Vector2 UpdateDelta(float targetVar, float currentVar, float deltaVar) {
            return targetVar * m_target + currentVar * m_current + deltaVar * m_delta;
        }

		protected override bool Insensitive () {
			return Vector2.Distance(m_current, m_target) < m_sensitivity;
        }
    }

	public class NumericSpring3 : NumericSpring<Vector3> {
        public NumericSpring3 (Vector3 origin, float frequency, float dumpingRatio = 1, float sensitivity = 0.01F) : base(origin, frequency, dumpingRatio, sensitivity) {}
        
        protected override Vector3 UpdateCurrent(float targetVar, float currentVar, float deltaVar) {
            return targetVar * m_target + currentVar * m_current + deltaVar * m_delta;
        }

        protected override Vector3 UpdateDelta(float targetVar, float currentVar, float deltaVar) {
            return targetVar * m_target + currentVar * m_current + deltaVar * m_delta;
        }

		protected override bool Insensitive () {
			return Vector3.Distance(m_current, m_target) < m_sensitivity;
        }
    }
}
