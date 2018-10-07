using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using DouduckLib.UI;

namespace DouduckLibEditor.UI {
    [CanEditMultipleObjects, CustomEditor (typeof (InteractableText), true)]
    public class InteractableTextEditor : GraphicEditor {

        [MenuItem ("GameObject/UI/Interactable Text")]
        static void Create (MenuCommand command) {
            var go = new GameObject ("InteractableText");
            go.AddComponent<InteractableText> ();
            GameObjectUtility.SetParentAndAlign (go, command.context as GameObject);
            Undo.RegisterCreatedObjectUndo (go, "Create" + go.name);
            Selection.activeObject = go;
        }

        GUIContent m_textContent;
        SerializedProperty m_nonParsedStr;
        SerializedProperty m_fontData;
        SerializedProperty m_raycastTarget;
        SerializedProperty m_onUrlClick;

        protected override void OnEnable () {
            base.OnEnable ();
            this.m_textContent = new GUIContent ("Text");
            this.m_nonParsedStr = base.serializedObject.FindProperty ("nonParsedStr");
            this.m_fontData = base.serializedObject.FindProperty ("m_FontData");
            this.m_raycastTarget = base.serializedObject.FindProperty ("m_RaycastTarget");
            this.m_onUrlClick = base.serializedObject.FindProperty ("onUrlClick");
        }

        public override void OnInspectorGUI () {
            base.serializedObject.Update ();
            EditorGUILayout.PropertyField (this.m_nonParsedStr, this.m_textContent);
            EditorGUILayout.PropertyField (this.m_fontData);
            base.AppearanceControlsGUI ();
            EditorGUILayout.PropertyField (this.m_raycastTarget);
            EditorGUILayout.PropertyField (this.m_onUrlClick);
            base.serializedObject.ApplyModifiedProperties ();
        }
    }
}