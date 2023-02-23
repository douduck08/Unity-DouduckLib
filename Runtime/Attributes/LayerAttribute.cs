﻿using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib {
    [AttributeUsage (AttributeTargets.Field)]
    public sealed class LayerAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer (typeof (LayerAttribute))]
    public class LayerPropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight (property, label, true);
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.Integer) {
                EditorGUI.HelpBox (position, string.Format ("[LayerAttribute] The field '{0}' must be int.", label.text), MessageType.Error);
            } else {
                property.intValue = EditorGUI.LayerField (position, label.text, property.intValue);
            }
        }
    }
#endif
}