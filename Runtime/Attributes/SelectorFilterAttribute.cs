using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SelectorFilterAttribute : PropertyAttribute
    {
        public string filter;
        public Type type;
        public SelectorFilterAttribute(string filter, Type type)
        {
            this.filter = filter;
            this.type = type;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SelectorFilterAttribute))]
    public class SelectorFilterDrawer : PropertyDrawer
    {
        // TODO: fix bug when multiple field in one component
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) * 2f;
        }

        int id;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SelectorFilterAttribute filterAttribute = attribute as SelectorFilterAttribute;
            if (filterAttribute.type != typeof(Sprite) && filterAttribute.type != typeof(Texture2D))
            {
                EditorGUI.HelpBox(position, string.Format("[SelectorFilter] The type is not supported.", label.text), MessageType.Error);
            }

            var fieldPos = position;
            fieldPos.xMax -= 30f;
            var btnPos = position;
            btnPos.xMin = fieldPos.xMax;
            property.objectReferenceValue = EditorGUI.ObjectField(fieldPos, label, property.objectReferenceValue, filterAttribute.type, false);

            id = property.propertyPath.GetHashCode();
            if (GUI.Button(btnPos, "⊙"))
            {
                if (filterAttribute.type == typeof(Sprite))
                {
                    EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, filterAttribute.filter, id);
                }
                else if (filterAttribute.type == typeof(Texture2D))
                {
                    EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, filterAttribute.filter, id);
                }
            }

            var evt = Event.current;
            if (evt.commandName == "ObjectSelectorUpdated" && id == EditorGUIUtility.GetObjectPickerControlID())
            {
                property.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
            }
        }
    }
#endif
}
