using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DouduckLib.UI;

namespace DouduckLibEditor.UI {
    public class MoreUIMenuItems {
        [MenuItem ("GameObject/UI/Others/Empty UI")]
        static void CreateEmptyUI (MenuCommand command) {
            var go = new GameObject ("EmptyUI");
            go.AddComponent<EmptyUI> ();
            GameObjectUtility.SetParentAndAlign (go, command.context as GameObject);
            Undo.RegisterCreatedObjectUndo (go, "Create" + go.name);
            Selection.activeObject = go;
        }

        [MenuItem ("GameObject/UI/Others/Non-Raycast Image", false)]
        static void CreatImage (MenuCommand menuCommand) {
            EditorApplication.ExecuteMenuItem ("GameObject/UI/Image");
            GameObject go = Selection.activeGameObject;
            GameObjectUtility.SetParentAndAlign (go, menuCommand.context as GameObject);
            go.GetComponent<Image> ().raycastTarget = false;
        }

        [MenuItem ("GameObject/UI/Others/Non-Raycast Text", false)]
        static void CreatText (MenuCommand menuCommand) {
            EditorApplication.ExecuteMenuItem ("GameObject/UI/Text");
            GameObject go = Selection.activeGameObject;
            GameObjectUtility.SetParentAndAlign (go, menuCommand.context as GameObject);
            go.GetComponent<Text> ().raycastTarget = false;
        }
    }
}