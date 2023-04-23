using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace DouduckLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayLabelAttribute : PropertyAttribute
    {
        public string label;
        public DisplayLabelAttribute(string label)
        {
            this.label = label;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisplayLabelAttribute))]
    public class DisplayLabelPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var displayLabelAttribute = attribute as DisplayLabelAttribute;
            label.text = displayLabelAttribute.label;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
#endif
}
