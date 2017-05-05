using UnityEngine;
using System.Collections;

public class LogDisplay : MonoBehaviour {
	public bool isOpen = true;
	public int FontSize = 24;

	private string myLog = "";
	private int btnSize = 30;
	private GUIStyle btnStyle = null;
	private GUIStyle labelStyle = null;

	void OnEnable () {
		Application.logMessageReceived += newLog;
	}

	void OnDisable () {
		// Remove callback when object goes out of scope
		Application.logMessageReceived -= newLog;
	}

	void InitGUI() {
		if (btnStyle == null) {
			int w = Screen.width, h = Screen.height;
			btnStyle = new GUIStyle (GUI.skin.button);
			btnStyle.fontSize = btnSize / 2;
		}
		if (labelStyle == null) {
			labelStyle = new GUIStyle ();
			labelStyle.alignment = TextAnchor.UpperLeft;
			labelStyle.fontSize = FontSize;
			labelStyle.normal.textColor = new Color (0.7f, 0.7f, 0.7f, 1.0f);
		}
	}

	void OnGUI () {
		InitGUI();
		int w = Screen.width, h = Screen.height;
		if (isOpen) {
			if (GUI.Button(new Rect(10, 10, btnSize, btnSize), "x", btnStyle)) {
				isOpen = false;
			}
			if (GUI.Button(new Rect(20 + btnSize , 10, btnSize, btnSize), "<", btnStyle)) {
				FontSize -= 4;
				labelStyle.fontSize = FontSize;
			}
			if (GUI.Button(new Rect(30 + btnSize * 2, 10, btnSize, btnSize), ">", btnStyle)) {
				FontSize += 4;
				labelStyle.fontSize = FontSize;
			}
			GUI.Label(new Rect(10, 10 + btnSize, Screen.width - 20, Screen.height - 20 - btnSize), myLog, labelStyle);
		} else {
			if (GUI.Button(new Rect(10, 10, btnSize, btnSize), "o", btnStyle)) {
				isOpen = true;
			}
		}
	}

	public void newLog(string logString, string stackTrace, LogType type)	{
		myLog = logString + "\n" + myLog;
		if (myLog.Length > 512) {
			// if myLog is too long, cut it.
			myLog = myLog.Substring(0, 374);
		}
	}

}