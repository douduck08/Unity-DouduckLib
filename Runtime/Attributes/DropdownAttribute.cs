using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DropdownAttribute : PropertyAttribute
    {

        public enum ValueType
        {
            Integer,
            Float,
            String
        }

        public ValueType valueType;
        public string[] displayNames;

        public int[] intValues;
        public float[] floatValues;
        public string[] stringValues;

        public DropdownAttribute(int[] values, string[] displayNames)
        {
            this.valueType = ValueType.Integer;
            this.displayNames = displayNames;
            this.intValues = values;
        }

        public int GetIndex(int value)
        {
            for (int i = 0; i < intValues.Length; i++)
            {
                if (intValues[i] == value)
                    return i;
            }
            return 0;
        }

        public DropdownAttribute(float[] values, string[] displayNames)
        {
            this.valueType = ValueType.Float;
            this.displayNames = displayNames;
            this.floatValues = values;
        }

        public int GetIndex(float value)
        {
            for (int i = 0; i < floatValues.Length; i++)
            {
                if (floatValues[i] == value)
                    return i;
            }
            return 0;
        }

        public DropdownAttribute(string[] values, string[] displayNames)
        {
            this.valueType = ValueType.String;
            this.displayNames = displayNames;
            this.stringValues = values;
        }

        public int GetIndex(string value)
        {
            for (int i = 0; i < intValues.Length; i++)
            {
                if (stringValues[i] == value)
                    return i;
            }
            return 0;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            DropdownAttribute dropdown = attribute as DropdownAttribute;

            if (dropdown.valueType.ToString() == property.propertyType.ToString())
            {
                if (dropdown.valueType == DropdownAttribute.ValueType.Integer)
                {
                    int index = dropdown.GetIndex(property.intValue);
                    index = EditorGUI.Popup(position, label.text, index, dropdown.displayNames);
                    property.intValue = dropdown.intValues[index];
                }
                else if (dropdown.valueType == DropdownAttribute.ValueType.Float)
                {
                    int index = dropdown.GetIndex(property.floatValue);
                    index = EditorGUI.Popup(position, label.text, index, dropdown.displayNames);
                    property.floatValue = dropdown.floatValues[index];
                }
                else if (dropdown.valueType == DropdownAttribute.ValueType.String)
                {
                    int index = dropdown.GetIndex(property.stringValue);
                    index = EditorGUI.Popup(position, label.text, index, dropdown.displayNames);
                    property.stringValue = dropdown.stringValues[index];
                }
            }
            else
            {
                EditorGUI.HelpBox(position, string.Format("[DropdownAttribute] The field '{0}' must match 'values'.", label.text), MessageType.Error);
            }
        }
    }
#endif
}
