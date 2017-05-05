using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public class NumericSpring3D {

		private Vector3 m_TargetValue;
		private Vector3 m_CurrentValue;
		private Vector3 m_CurrentChange;

		private float m_fDumpingRatio;
		private float m_fFrequency;
		private float m_fSensitivity;

		public NumericSpring3D (Vector3 oriValue, float fFrequency, float fDumpingRatio = 1.0f, float fSensitivity = 0.1f) {
			m_TargetValue = oriValue;
			m_CurrentValue = oriValue;
			m_CurrentChange = new Vector3();

			m_fFrequency = fFrequency;
			m_fDumpingRatio = fDumpingRatio;
			m_fSensitivity = fSensitivity;
		}


		public void SetTargetValue(Vector3 TargetValue) {
			m_TargetValue = TargetValue;
		}

		public Vector3 DumpingUpdate(float fDeltaTime) {
			float ww = m_fFrequency * m_fFrequency;
			float wwt = ww * fDeltaTime;
			float wwtt = wwt * fDeltaTime;
			float f = 1.0f + 2.0f * fDeltaTime * m_fDumpingRatio * m_fFrequency;
			float detInv = 1.0f / (f + wwtt);

			m_CurrentValue = (m_CurrentValue * f + wwtt * m_TargetValue + fDeltaTime * m_CurrentChange) * detInv;
			m_CurrentChange = (m_CurrentChange + wwt * (m_TargetValue - m_CurrentValue)) * detInv;

			if (Vector3.Magnitude(m_CurrentValue - m_TargetValue) < m_fSensitivity) {
				m_CurrentValue = m_TargetValue;
			}
			return m_CurrentValue;
		}
	}
}
