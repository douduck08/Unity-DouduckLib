using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DouduckLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayNameAttribute : PropertyAttribute
    {
        public string label;
        public DisplayNameAttribute(string label)
        {
            this.label = label;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisplayNameAttribute))]
    public class DisplayNamePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var displayLabelAttribute = attribute as DisplayNameAttribute;
            label.text = displayLabelAttribute.label;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
#endif
}
