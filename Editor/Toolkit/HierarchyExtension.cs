using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using DouduckLib;

namespace DouduckLibEditor {
    [InitializeOnLoad]
    public class HierarchyExtension {

        static Dictionary<int, Canvas> m_markedCanvas = new Dictionary<int, Canvas> ();
        static Dictionary<int, Graphic> m_markedGraphic = new Dictionary<int, Graphic> ();

        static bool showCanvasToggle;
        const string showCanvasTogglePath = "Tools/Hierarchy Extension/Show UI Canvas Toggle";

        static bool showGraphicToggle;
        const string showGraphicTogglePath = "Tools/Hierarchy Extension/Show UI Graphic Toggle";

        static HierarchyExtension () {
            EditorApplication.update += HierarchyUpdate;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
        }

        [MenuItem (showCanvasTogglePath, false, 100)]
        static void ShowCanvasToggle () {
            showCanvasToggle = !showCanvasToggle;
            if (showCanvasToggle) {
                showGraphicToggle = false;
            }

            Menu.SetChecked (showCanvasTogglePath, showCanvasToggle);
            Menu.SetChecked (showGraphicTogglePath, showGraphicToggle);
        }

        [MenuItem (showGraphicTogglePath, false, 101)]
        static void ShowGraphicToggle () {
            showGraphicToggle = !showGraphicToggle;
            if (showGraphicToggle) {
                showCanvasToggle = false;
            }

            Menu.SetChecked (showCanvasTogglePath, showCanvasToggle);
            Menu.SetChecked (showGraphicTogglePath, showGraphicToggle);
        }

        static void HierarchyUpdate () {
            m_markedCanvas.Clear ();
            foreach (var item in UnityUtil.GetAllComponents<Canvas> ()) {
                m_markedCanvas.Add (item.gameObject.GetInstanceID (), item);
            }

            m_markedGraphic.Clear ();
            foreach (var item in UnityUtil.GetAllComponents<Graphic> ()) {
                m_markedGraphic.Add (item.gameObject.GetInstanceID (), item);
            }
        }

        static void HierarchyOnGUI (int instanceID, Rect selectionRect) {
            var rect = new Rect (selectionRect);
            rect.x = 1;
            rect.width = 18;

            if (showCanvasToggle) {
                DrawToggle (rect, instanceID, m_markedCanvas);
            }
            if (showGraphicToggle) {
                DrawToggle (rect, instanceID, m_markedGraphic);
            }
        }

        static void DrawToggle<T> (Rect rect, int instanceID, Dictionary<int, T> markedObjects) where T : Behaviour {
            if (markedObjects.ContainsKey (instanceID) && markedObjects[instanceID] != null) {
                EditorGUI.BeginChangeCheck ();
                markedObjects[instanceID].enabled = GUI.Toggle (rect, markedObjects[instanceID].enabled, "");
                if (EditorGUI.EndChangeCheck ()) {
                    EditorSceneManager.MarkSceneDirty (SceneManager.GetActiveScene ());
                }
            }
        }
    }
}