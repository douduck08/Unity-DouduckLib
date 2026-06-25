using UnityEngine;
using UnityEditor;
using DouduckLib.Localization;
using DouduckLibEditor;

namespace DouduckLibEditor.Localization
{
    [CustomPropertyDrawer(typeof(LocalizedString))]
    public class LocalizedStringDrawer : PropertyDrawer
    {
        static bool previewEnabled;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // initialize variables
            var keyProp = property.FindPropertyRelative("_fullKeyOrText");
            var localizedProp = property.FindPropertyRelative("_isLocalized");
            var isLocalized = localizedProp.boolValue;

            // start drawing property
            const float spaceWidth = 2f;
            const float buttonWidth = 20f;

            EditorGUI.BeginProperty(position, label, property);
            var indentLevel = EditorGUI.indentLevel;

            var fieldPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive, position), label);
            EditorGUI.indentLevel = 0;

            var keyWidth = fieldPosition.width - buttonWidth * 3;
            var keyRect = new Rect(fieldPosition.x, position.y, keyWidth - spaceWidth, position.height);
            var localizedRect = new Rect(keyRect.xMax + spaceWidth, position.y, buttonWidth, position.height);
            var previewRect = new Rect(localizedRect.xMax, position.y, buttonWidth, position.height);
            var buttonRect = new Rect(previewRect.xMax, position.y, buttonWidth, position.height);

            // 1. Key TextField - Handle multi-selection mixed values
            GUI.enabled = !previewEnabled;
            if (previewEnabled)
            {
                EditorGUI.showMixedValue = keyProp.hasMultipleDifferentValues;
                var previewText = keyProp.hasMultipleDifferentValues ? "" : DouduckLib.Localization.Localization.Get().GetString(keyProp.stringValue);
                EditorGUI.TextField(keyRect, previewText);
                EditorGUI.showMixedValue = false;
            }
            else
            {
                EditorGUI.showMixedValue = keyProp.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                var newKeyValue = EditorGUI.TextField(keyRect, keyProp.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    keyProp.stringValue = newKeyValue;
                }
                EditorGUI.showMixedValue = false;
            }

            // 2. Localized Toggle ("L" button) - Handle multi-selection mixed values
            GUI.enabled = true;
            EditorGUI.showMixedValue = localizedProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var newIsLocalized = GUI.Toggle(localizedRect, isLocalized, new GUIContent("L"), EditorStyles.miniButton);
            if (EditorGUI.EndChangeCheck())
            {
                localizedProp.boolValue = newIsLocalized;
            }
            EditorGUI.showMixedValue = false;

            // 3. Preview Toggle ("P" button)
            GUI.enabled = localizedProp.boolValue && !localizedProp.hasMultipleDifferentValues;
            if (GUI.Toggle(previewRect, previewEnabled, new GUIContent("P"), EditorStyles.miniButton) != previewEnabled)
            {
                previewEnabled = !previewEnabled;
            }

            // 4. Popup Button (Dropdown Menu)
            GUI.enabled = localizedProp.boolValue && !localizedProp.hasMultipleDifferentValues;
            if (GUI.Button(buttonRect, "", EditorStyles.popup))
            {
                var menu = new SearchableGenericMenu();
                foreach (string fullkey in DouduckLib.Localization.Localization.Get().GetAllKeys())
                {
                    var menuPath = fullkey.Replace('.', '/');
                    menu.AddItem(new GUIContent(menuPath), false, OnSelectKey, new MenuItemData(property.serializedObject, keyProp, fullkey));
                }
                if (menu.GetItemCount() == 0)
                {
                    menu.AddDisabledItem(new GUIContent("(none)"), false);
                }
                menu.DropDown(buttonRect);
            }
            GUI.enabled = true;

            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndProperty();
        }

        void OnSelectKey(MenuItemData data)
        {
            data.serializedObject.Update();
            data.keyProperty.stringValue = data.selectedKey;
            data.serializedObject.ApplyModifiedProperties();
        }

        class MenuItemData
        {
            public SerializedObject serializedObject;
            public SerializedProperty keyProperty;
            public string selectedKey;

            public MenuItemData(SerializedObject obj, SerializedProperty prop, string key)
            {
                serializedObject = obj;
                keyProperty = prop;
                selectedKey = key;
            }
        }
    }
}
