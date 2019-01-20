using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib {
    [AttributeUsage (AttributeTargets.Field)]
    public class SelectorFilterAttribute : PropertyAttribute {
        public static string currentActiveFieldName;
        public string filter;
        public Type type;
        public SelectorFilterAttribute (string filter, Type type) {
            this.filter = filter;
            this.type = type;
        }
    }

#if UNITY_EDITOR
    // public static class ObjectSelectorWrapper {
    //     static System.Type selectorType;
    //     static FieldInfo sharedSelectorFieldInfo;
    //     static FieldInfo filterFieldInfo;
    //     static PropertyInfo isVisiblePropertyInfo;
    //     static bool oldState;

    //     static ObjectSelectorWrapper () {
    //         selectorType = System.Type.GetType ("UnityEditor.ObjectSelector,UnityEditor");
    //         sharedSelectorFieldInfo = selectorType.GetField ("s_SharedObjectSelector", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
    //         filterFieldInfo = selectorType.GetField ("m_SearchFilter", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //         isVisiblePropertyInfo = selectorType.GetProperty ("isVisible", BindingFlags.Public | BindingFlags.Static);
    //     }

    //     public static bool isVisible {
    //         get {
    //             return (bool) isVisiblePropertyInfo.GetValue (null, null);
    //         }
    //     }

    //     public static bool justBeOpened {
    //         get {
    //             bool value = isVisible;
    //             if (value && !oldState) {
    //                 oldState = value;
    //                 return true;
    //             }
    //             oldState = value;
    //             return false;
    //         }
    //     }

    //     public static EditorWindow GetObjectSelectorWindow () {
    //         return sharedSelectorFieldInfo.GetValue (null) as EditorWindow;
    //     }

    //     public static void SetFilter (string filter) {
    //         if (filter == null) filter = String.Empty;
    //         Debug.Log (GetObjectSelectorWindow () + ": " + filter);
    //         filterFieldInfo.SetValue (GetObjectSelectorWindow (), filter);
    //     }
    // }

    [CustomPropertyDrawer (typeof (SelectorFilterAttribute))]
    public class SelectorFilterDrawer : PropertyDrawer {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight (property, label, true) * 2f;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            SelectorFilterAttribute filterAttribute = attribute as SelectorFilterAttribute;

            var fieldPos = position;
            fieldPos.xMax -= 30f;
            var btnPos = position;
            btnPos.xMin = fieldPos.xMax;

            property.objectReferenceValue = EditorGUI.ObjectField (fieldPos, label, property.objectReferenceValue, filterAttribute.type, false);

            if (GUI.Button (btnPos, "⊙")) {
                int controlID = EditorGUIUtility.GetControlID (FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Sprite> (null, false, filterAttribute.filter, controlID);
                SelectorFilterAttribute.currentActiveFieldName = property.name;
            }

            string commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorUpdated" && SelectorFilterAttribute.currentActiveFieldName == property.name) {
                property.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject ();
                SelectorFilterAttribute.currentActiveFieldName = "";
            }
        }
    }
#endif
}