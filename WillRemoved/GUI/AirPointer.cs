/*
@file uGUIAirPointer.cs
@author Leo
@date 20160713 . Created
*/

using UnityEngine;
using System.Collections;
using DouduckGame;

public class AirPointer : IGUIComponent {

	public float LOOPTIME = 0.2f;
	private float fLoopTime;

	private RotationDetector m_Rotation;
	private Vector2 m_Pointer;

	void OnEnable () {
		m_Rotation = new RotationDetector (3);
		m_Rotation.Reset();
		fLoopTime = LOOPTIME;
	}

	void Update () {
		if (fLoopTime > 0) {
			fLoopTime -= Time.deltaTime;
		} else {
			m_Pointer = m_Rotation.GetProjection();
			// UpperUnit.EndDrag(Vecter2ToDegree(m_Pointer), m_Pointer.magnitude);
			UpperUnit.EndDrag(m_Pointer.x, m_Pointer.y);

			fLoopTime = LOOPTIME;
			m_Rotation.Reset();
		}
	}

	private int Vecter2ToDegree(Vector2 vVec) {
		if (vVec.y >= 0) {
			return (int)Vector2.Angle(Vector2.right, vVec);
		} else {
			return 360 - (int)Vector2.Angle(Vector2.right, vVec);
		}
	}
}
