using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public class NumericSpring {

		private float m_TargetValue;
		private float m_CurrentValue;
		private float m_CurrentChange;

		private float m_fDumpingRatio;
		private float m_fFrequency;
		private float m_fSensitivity;

		public NumericSpring (float oriValue, float fFrequency, float fDumpingRatio = 1.0f, float fSensitivity = 0.1f) {
			m_TargetValue = oriValue;
			m_CurrentValue = oriValue;
			m_CurrentChange = 0;

			m_fFrequency = fFrequency;
			m_fDumpingRatio = fDumpingRatio;
			m_fSensitivity = fSensitivity;
		}


		public void SetTargetValue(float TargetValue) {
			m_TargetValue = TargetValue;
		}

		public float DumpingUpdate(float fDeltaTime) {
			float ww = m_fFrequency * m_fFrequency;
			float wwt = ww * fDeltaTime;
			float wwtt = wwt * fDeltaTime;
			float f = 1.0f + 2.0f * fDeltaTime * m_fDumpingRatio * m_fFrequency;
			float detInv = 1.0f / (f + wwtt);

			m_CurrentValue = (m_CurrentValue * f + wwtt * m_TargetValue + fDeltaTime * m_CurrentChange) * detInv;
			m_CurrentChange = (m_CurrentChange + wwt * (m_TargetValue - m_CurrentValue)) * detInv;

			if (Mathf.Abs(m_CurrentValue - m_TargetValue) < m_fSensitivity) {
				m_CurrentValue = m_TargetValue;
			}
			return m_CurrentValue;
		}
	}
}
