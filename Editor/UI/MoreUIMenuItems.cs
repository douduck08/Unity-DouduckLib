using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DouduckLib.UI;

namespace DouduckLibEditor.UI
{
    public class MoreUIMenuItems
    {
        const string uiMenuItemPath = "GameObject/UI/Others/";

        [MenuItem(uiMenuItemPath + "Empty Graphic")]
        static void CreateEmptyGraphic(MenuCommand command)
        {
            var go = new GameObject("EmptyGraphic");
            go.AddComponent<EmptyGraphic>();
            GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create" + go.name);
            Selection.activeObject = go;
        }

        [MenuItem(uiMenuItemPath + "Interactable Text")]
        static void CreateInteractableText(MenuCommand command)
        {
            var go = new GameObject("InteractableText");
            go.AddComponent<InteractableText>();
            GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create" + go.name);
            Selection.activeObject = go;
        }

        [MenuItem(uiMenuItemPath + "Non-Raycast Image")]
        static void CreatImage(MenuCommand menuCommand)
        {
            EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
            GameObject go = Selection.activeGameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            go.GetComponent<Image>().raycastTarget = false;
        }

        [MenuItem(uiMenuItemPath + "Non-Raycast Text")]
        static void CreatText(MenuCommand menuCommand)
        {
            EditorApplication.ExecuteMenuItem("GameObject/UI/Text");
            GameObject go = Selection.activeGameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            go.GetComponent<Text>().raycastTarget = false;
        }
    }
}