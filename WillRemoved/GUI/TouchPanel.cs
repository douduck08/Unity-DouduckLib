/*
@file uGUITouchPanel.cs
@author Leo
@date 20160713 . Created
*/

using UnityEngine;
using System.Collections;
using DouduckGame;

public class TouchPanel : IGUIComponent {

	// TODO: public bool EnableDrag = true;
	public bool EnableClick = true;
	public float SENSITIVITY = 0.0f;
	public float COLDDOWN = 0.5f;

	private float fColdDownTime;
	private bool  m_bIsOnDrag;
	private Vector2 m_oBeginPos;
	private Vector2 m_oEndPos;
	private Vector2 m_oDiffPos;

	void OnEnable () {
		UnityEngine.UI.Image img = transform.GetComponent<UnityEngine.UI.Image>();
		if (img != null) {
			img.raycastTarget = true;
		}

		m_bIsOnDrag = false;
		EventTriggerListener.Get(transform.gameObject).onBeginDrag = OnBeginDrag;
		EventTriggerListener.Get(transform.gameObject).onEndDrag = OnEndDrag;
	}

	void Update () {
		if (fColdDownTime > 0) {
			fColdDownTime -= Time.deltaTime;
		}
	}

	private void OnBeginDrag (GameObject go)
	{
		m_bIsOnDrag = true;
#if UNITY_EDITOR
		// TODO: mouse version
#else
		m_oBeginPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
#endif
	}

	private void OnEndDrag (GameObject go) {
		m_bIsOnDrag = false;
#if UNITY_EDITOR
#else
		m_oEndPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
#endif
		if (fColdDownTime <= 0.0f) {
			fColdDownTime = COLDDOWN;
			m_oDiffPos = m_oEndPos - m_oBeginPos;
			if (m_oDiffPos.magnitude > SENSITIVITY) {
				// UpperUnit.EndDrag(Vecter2ToDegree(m_oDiffPos), m_oDiffPos.magnitude);
				UpperUnit.EndDrag(m_oDiffPos.x, m_oDiffPos.y);
			} else if (EnableClick) {
				UpperUnit.ButtonClick(Name);
			}
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
