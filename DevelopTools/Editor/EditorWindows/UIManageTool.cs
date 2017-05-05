using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIManageTool : EditorWindow {

	private Canvas m_canvas = null;
	private GameObject[] m_UIObjects = null;
	private bool[] m_UIObjectsActive = null;

	[MenuItem("DDTool/UI ManageTool", false, 20)]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(UIManageTool), false, "UI Manager");
	}

	public void OnFocus() {
		if (m_canvas == null) {
			GameObject go_ = GameObject.Find("Canvas");
			if (go_ != null) {
				m_canvas = go_.GetComponent<Canvas>();
			}
		}
		if (m_canvas != null) {
			Refresh();
		}
	}

	public void OnGUI() {
		m_canvas = (Canvas)EditorGUILayout.ObjectField("Root Canvas", m_canvas, typeof(Canvas), true);

		if (m_UIObjects != null && m_UIObjectsActive != null) {
			for (int i = 0; i < m_UIObjects.Length; i++) {
				if (m_UIObjects [i] == null) {
					continue;
				}
				m_UIObjectsActive[i] = EditorGUILayout.ToggleLeft(m_UIObjects[i].name, m_UIObjects[i].activeSelf);
				if (m_UIObjectsActive[i] != m_UIObjects [i].activeSelf) {
					m_UIObjects [i].SetActive(m_UIObjectsActive [i]);
				}
			}

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Active All")) {
				ActiveAll();
			}
			if (GUILayout.Button("Inactive All")) {
				InactiveAll();
			}
			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Refresh")) {
			Refresh();
		}
	}

	private void Refresh() {
		if (m_canvas != null) {
			m_UIObjects = new GameObject[m_canvas.transform.childCount];
			m_UIObjectsActive = new bool[m_canvas.transform.childCount];
			int i = 0;
			foreach (Transform childT in m_canvas.transform) {
				m_UIObjects[i] = childT.gameObject;
				m_UIObjectsActive [i] = childT.gameObject.activeSelf;
				i++;
			}
		} else {
			m_UIObjects = null;
			m_UIObjectsActive = null;
		}
	}

    private void SetActiveAndRecord (GameObject go, bool activeValue) {
        if (go.activeSelf != activeValue) {
            Undo.RecordObject (go, "SetActive");
            go.SetActive (activeValue);
        }
    }

    private void ActiveAll () {
        for (int i = 0; i < m_UIObjects.Length; i++) {
            SetActiveAndRecord (m_UIObjects[i], true);
        }
    }

    private void InactiveAll () {
        for (int i = 0; i < m_UIObjects.Length; i++) {
            SetActiveAndRecord (m_UIObjects[i], false);
        }
    }
}
