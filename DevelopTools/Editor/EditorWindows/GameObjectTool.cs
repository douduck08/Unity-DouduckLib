using UnityEngine;
using UnityEditor;

public class GameObjectTool : EditorWindow {
	
	private bool bFoldSetActive, bFoldFindMissing;
	private int go_count = 0, components_count = 0, missing_count = 0;
	
	[MenuItem("DDTool/GameObject ManageTool", false, 20)]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(GameObjectTool), false, "GameObject Tool");
	}
	
	public void OnGUI() {
		bFoldSetActive = EditorGUILayout.Foldout(bFoldSetActive, "Set Active to Selection");
		if (bFoldSetActive) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Active Selection")) {
				SetActiveInSelected(true);
			}
			if (GUILayout.Button("Inactive Selection")) {
				SetActiveInSelected(false);
			}
			EditorGUILayout.EndHorizontal();
		}

	 	bFoldFindMissing = EditorGUILayout.Foldout(bFoldFindMissing, "Find Missing Scripts");
		if (bFoldFindMissing) {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Find Missing Scripts", GUILayout.ExpandWidth(false), GUILayout.Height(50))) {
				FindInSelected();
			}
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Searched GameObjects:", go_count.ToString());
			EditorGUILayout.LabelField("Found Components:", components_count.ToString());
			EditorGUILayout.LabelField("Found Missing:", missing_count.ToString());
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
	}

	private void FindInSelected() {
		GameObject[] go = Selection.gameObjects;
		go_count = 0;
		components_count = 0;
		missing_count = 0;
		foreach (GameObject g in go) {
			FindInGO(g);
		}
	}
	
	private void FindInGO(GameObject g) {
		go_count++;
		Component[] components = g.GetComponents<Component>();
		for (int i = 0; i < components.Length; i++) {
			components_count++;
			if (components[i] == null) {
				missing_count++;
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null) 
				{
					s = t.parent.name +"/"+s;
					t = t.parent;
				}
				Debug.LogWarning (s + " has an empty script attached in position: " + i);
			}
		}
		foreach (Transform childT in g.transform) {
			FindInGO(childT.gameObject);
		}
	}

	private void SetActiveInSelected(bool active) {
		GameObject[] go = Selection.gameObjects;
		foreach (GameObject g in go) {
			SetActiveInGO(g, active);
		}
	}

	private void SetActiveInGO(GameObject g, bool active) {
		foreach (Transform childT in g.transform) {
			SetActiveInGO(childT.gameObject, active);
		}
		g.SetActive(active);
	}
}