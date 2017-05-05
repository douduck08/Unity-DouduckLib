using UnityEngine;
using System.Collections;

namespace DouduckGame {
	public class RotationDetector {

		private Quaternion m_oOriInverse = Quaternion.identity;
		private Quaternion m_oGyro;
		private Vector3 m_oPointer;
		private float m_fRange;

		public RotationDetector(float Range) {
			m_fRange = Range;
			if (SystemInfo.supportsGyroscope) {
				Input.gyro.enabled = true;
                Util.UnityConsole.Log("[RotationDetector] Gyro Enabled");
			}
		}

		public void Reset() {
            // UnityConsole.Log("[RotationDetector] Reset Original Direction");
            m_oOriInverse = Quaternion.Inverse(Input.gyro.attitude);

		}

		public Vector3 GetPointer() {
			m_oGyro = m_oOriInverse * Input.gyro.attitude;
			m_oPointer = Vector3.up;
			m_oPointer = m_oGyro * m_oPointer;
			return m_oPointer;
		}

		public Vector2 GetProjection() {
			GetPointer();

			if (m_oPointer.y < 0.1f) {
				return Vector2.zero;
			}

			float x_, z_;
			if (m_oPointer.x < m_oPointer.y * -m_fRange) {
				x_ = -m_fRange;
			} else if (m_oPointer.x > m_oPointer.y * m_fRange) {
				x_ = m_fRange;
			} else {
				x_ = m_oPointer.x / m_oPointer.y;
			}
			if (m_oPointer.z < m_oPointer.y * -m_fRange) {
				z_ = -m_fRange;
			} else if (m_oPointer.z > m_oPointer.y * m_fRange) {
				z_ = m_fRange;
			} else {
				z_ = m_oPointer.z / m_oPointer.y;
			}
			return new Vector2 (x_, z_);
		}

	}
}
