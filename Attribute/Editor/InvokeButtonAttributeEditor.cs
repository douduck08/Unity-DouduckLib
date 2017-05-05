using UnityEngine;
using UnityEditor;
using System.Reflection;

[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoBehaviourCustomEditor : UnityEditor.Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		if (Application.isPlaying) {
			var type = target.GetType();
			foreach (var method in type.GetMethods(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance)) {
				// make sure it is decorated by our custom attribute
				var attributes = method.GetCustomAttributes(typeof(InvokeButtonAttribute), true);
				if (attributes.Length > 0) {
					if (GUILayout.Button("Run: " + method.Name)) {
						// If the user clicks the button, invoke the method immediately.
						// There are many ways to do this but I chose to use Invoke which only works in Play Mode.
						((MonoBehaviour)target).Invoke(method.Name, 0f);
					}
				}
			}
		}
	}
}
