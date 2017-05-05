/*
@file GUIButton.cs
@author Leo
@date 20160315 . created
*/

using UnityEngine;
using System.Collections;
using DouduckGame;

public class NormalButton : IGUIComponent {

	public float COLDDOWN = 0.5f;
	private float fColdDownTime;

	void OnEnable () {
		UnityEngine.UI.Image img = transform.GetComponent<UnityEngine.UI.Image>();
		if (img != null) {
			img.raycastTarget = true;
		}

		EventTriggerListener.Get(transform.gameObject).onClick = OnButtonClick;
	}

	void Update () {
		if (fColdDownTime > 0) {
			fColdDownTime -= Time.deltaTime;
		}
	}

	private void OnButtonClick (GameObject go) {
		if (fColdDownTime <= 0.0f) {
			this.UpperUnit.ButtonClick(this.name);
			fColdDownTime = COLDDOWN;
		}
	}

    //	[InvokeButtonAttribute]
    //	public void TestPos() {
    //		UnityConsole.Log(transform.position);
    //	}
}
