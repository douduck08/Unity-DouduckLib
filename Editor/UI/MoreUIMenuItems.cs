using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DouduckLib.UI;

namespace DouduckLibEditor.UI
{
    public class MoreUIMenuItems
    {
        const string uiMenuItemPath = "GameObject/UI/Others/";

        static void CreateObjectWithComponent<T>(MenuCommand menuCommand, string name) where T : Component
        {
            var go = new GameObject(name);
            go.AddComponent<T>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem(uiMenuItemPath + "Empty Graphic")]
        static void CreateEmptyGraphic(MenuCommand menuCommand) => CreateObjectWithComponent<EmptyGraphic>(menuCommand, "EmptyGraphic");

        [MenuItem(uiMenuItemPath + "Vertical Group")]
        static void CreateVerticalGroup(MenuCommand menuCommand) => CreateObjectWithComponent<VerticalLayoutGroup>(menuCommand, "VerticalGroup");

        [MenuItem(uiMenuItemPath + "Horizontal Group")]
        static void CreateHorizontalGroup(MenuCommand menuCommand) => CreateObjectWithComponent<HorizontalLayoutGroup>(menuCommand, "HorizontalGroup");

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
