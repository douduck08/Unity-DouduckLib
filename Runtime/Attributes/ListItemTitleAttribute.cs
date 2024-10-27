using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ListItemTitleAttribute : PropertyAttribute
    {
        public string titleFieldName;
        public ListItemTitleAttribute(string titleField)
        {
            titleFieldName = titleField;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ListItemTitleAttribute))]
    public class ListItemTitlePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var titleAttribute = attribute as ListItemTitleAttribute;
            if (titleAttribute != null)
            {
                var titleProperty = property.FindPropertyRelative(titleAttribute.titleFieldName);
                if (titleProperty != null)
                {
                    label.text = GetString(titleProperty);
                }
            }
            EditorGUI.PropertyField(position, property, label, true);
        }

        static string GetString(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return property.floatValue.ToString();
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Enum:
                    return property.enumNames[property.enumValueIndex];
                default:
                    return "Not supported Types: " + property.propertyType.ToString();
            }
        }
    }
#endif
}
