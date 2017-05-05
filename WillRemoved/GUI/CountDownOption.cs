using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DouduckGame;

public class CountDownOption : IGUIComponent {

	public Text CountDownText;
	[ReadOnly]
	public bool IsActive = false;

	private Vector3 m_CloseScale = new Vector3(0, 1, 1);
	private Vector3 m_OpenScale = Vector3.one;
	private float m_fTimer;

	void OnEnable () {
		UnityEngine.UI.Image img = transform.GetComponent<UnityEngine.UI.Image>();
		if (img != null) {
			img.raycastTarget = true;
		}

		EventTriggerListener.Get(transform.gameObject).onClick = BeChosen;
	}

	void Update () {
		if (IsActive) {
			if (m_fTimer < 0) {
				IsActive = false;
				BeChosen(this.gameObject);
			} else {
				m_fTimer -= Time.deltaTime;

				CountDownText.text = string.Format("{0:#}", (int)(m_fTimer + 1f));
				float fPart = m_fTimer % 1f;
				if (fPart > 0.5f) {
					CountDownText.transform.localScale = Vector3.Lerp(m_CloseScale, m_OpenScale, (1f - fPart) * 2f);
				} else {
					CountDownText.transform.localScale = Vector3.Lerp(m_OpenScale, m_CloseScale, (0.5f - fPart) * 2f);
				}
			}
		}
	}

	public void StartCountDown () {
		if (IsActive == false) {
			IsActive = true;
			m_fTimer = 3f;
		}
	}

	public void StopCountDown () {
		IsActive = false;
		CountDownText.text = "";
	}

	private void BeChosen(GameObject go) {
		this.UpperUnit.ButtonClick(this.name);
	}
}
