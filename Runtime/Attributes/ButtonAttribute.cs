using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DouduckLib
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ButtonAttribute : PropertyAttribute
    {

        public string name;
        public bool enableInPlayMode;
        public bool enableInEditMode;

        public ButtonAttribute(string name, bool enableInPlayMode = true, bool enableInEditMode = true)
        {
            this.name = name;
            this.enableInPlayMode = enableInPlayMode;
            this.enableInEditMode = enableInEditMode;
        }

        public ButtonAttribute(bool enableInPlayMode = true, bool enableInEditMode = true)
        {
            this.enableInPlayMode = enableInPlayMode;
            this.enableInEditMode = enableInEditMode;
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects, CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonMonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawButtons();
        }

        protected void DrawButtons()
        {
            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                .Where(m => m.GetParameters().Length == 0);

            foreach (var method in methods)
            {
                var button = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
                if (button != null)
                {
                    string buttonName = string.IsNullOrEmpty(button.name) ? method.Name : button.name;

                    bool enabledCache = GUI.enabled;
                    GUI.enabled = Application.isPlaying ? button.enableInPlayMode : button.enableInEditMode;

                    if (GUILayout.Button(buttonName))
                    {
                        foreach (var t in targets)
                        {
                            method.Invoke(t, null);
                        }
                    }

                    GUI.enabled = enabledCache;
                }

            }
        }
    }

    [CanEditMultipleObjects, CustomEditor(typeof(ScriptableObject), true)]
    public class ButtonScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawButtons();
        }

        protected void DrawButtons()
        {
            var methods = this.target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetParameters().Length == 0);

            foreach (var method in methods)
            {
                var button = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
                if (button != null)
                {
                    string buttonName = string.IsNullOrEmpty(button.name) ? method.Name : button.name;

                    bool enabledCache = GUI.enabled;
                    GUI.enabled = Application.isPlaying ? button.enableInPlayMode : button.enableInEditMode;

                    if (GUILayout.Button(buttonName))
                    {
                        foreach (var t in targets)
                        {
                            method.Invoke(t, null);
                        }
                    }

                    GUI.enabled = enabledCache;
                }

            }
        }
    }
#endif
}
