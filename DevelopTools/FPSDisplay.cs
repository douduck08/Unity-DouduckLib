using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {
	private float deltaTime = 0.0f;
	private GUIStyle style;
	private Rect rect;

	void Awake()
	{
		int w = Screen.width, h = Screen.height;
		rect = new Rect(0, 0, w, h * 2 / 100);
		style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
	}

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}