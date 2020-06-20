using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib {
    [AttributeUsage (AttributeTargets.Field)]
    public class SceneAssetAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer (typeof (SceneAssetAttribute))]
    public class SceneAssetPropertyDrawer : PropertyDrawer {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight (property, label, true);
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.HelpBox (position, string.Format ("[SceneNameAttribute] The field '{0}' must be string.", label.text), MessageType.Error);
            } else {
                var scenePath = property.stringValue;
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset> (scenePath);

                EditorGUI.BeginChangeCheck ();
                sceneAsset = EditorGUI.ObjectField (position, label, sceneAsset, typeof (SceneAsset), false) as SceneAsset;
                if (EditorGUI.EndChangeCheck ()) {
                    var newPath = AssetDatabase.GetAssetPath (sceneAsset);
                    property.stringValue = newPath;
                }
            }
        }
    }
#endif
}