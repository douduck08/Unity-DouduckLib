using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DouduckGame;

public class GameObjectSetManager : EditorWindow {

    private GameObject[] m_UIObjects = null;
    private bool[] m_UIObjectsActive = null;

    [MenuItem("DDTool/GameObjectSet Manager", false, 20)]
    public static void ShowWindow () {
        EditorWindow.GetWindow (typeof (GameObjectSetManager), false, "GameObjectSet Manager");
    }

    public void OnGUI () {
        if (m_UIObjects != null && m_UIObjectsActive != null) {
            for (int i = 0; i < m_UIObjects.Length; i++) {
                if (m_UIObjects[i] == null) {
                    continue;
                }
                m_UIObjectsActive[i] = EditorGUILayout.ToggleLeft (m_UIObjects[i].name, m_UIObjects[i].activeSelf);
                if (m_UIObjectsActive[i] != m_UIObjects[i].activeSelf) {
                    m_UIObjects[i].SetActive (m_UIObjectsActive[i]);
                }
            }

            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("Active All")) {
                ActiveAll ();
            }
            if (GUILayout.Button ("Inactive All")) {
                InactiveAll ();
            }
            EditorGUILayout.EndHorizontal ();
        }

        if (GUILayout.Button ("Refresh")) {
            Refresh ();
        }
    }

    private void Refresh () {
        GameObjectSet[] sets = GameObject.FindObjectsOfType<GameObjectSet> ();
        if (sets.Length > 0) {
            m_UIObjects = new GameObject[sets.Length];
            m_UIObjectsActive = new bool[sets.Length];
            for (int i = 0; i < sets.Length; i++) {
                m_UIObjects[i] = sets[i].gameObject;
                m_UIObjectsActive[i] = sets[i].gameObject.activeSelf;
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
