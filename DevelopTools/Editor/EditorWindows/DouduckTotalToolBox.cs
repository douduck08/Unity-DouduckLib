using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DouduckGame;

public class DouduckTotalToolBox : EditorWindow {

	[MenuItem("DDTool/Total Tool Box", false, 0)]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(DouduckTotalToolBox), false, "Unity Tool Box");
	}

	public void OnGUI() {
		if (GUILayout.Button("Game Object Tool")) {
			GameObjectTool.ShowWindow();
		}

		if (GUILayout.Button("Test")) {

		}
	}
}
