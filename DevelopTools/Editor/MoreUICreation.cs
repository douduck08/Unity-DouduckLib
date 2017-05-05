using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MoreUICreation {

	[MenuItem("GameObject/More UI/Non-Raycast Image", false, 10)]
	static void CreatImage(MenuCommand menuCommand)
	{
		EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
		GameObject go = Selection.activeGameObject;
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		go.GetComponent<Image>().raycastTarget = false;
	}

	[MenuItem("GameObject/More UI/Non-Raycast Text", false, 20)]
	static void CreatText(MenuCommand menuCommand)
	{
		EditorApplication.ExecuteMenuItem("GameObject/UI/Text");
		GameObject go = Selection.activeGameObject;
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		go.GetComponent<Text>().raycastTarget = false;
	}

    [MenuItem ("GameObject/More UI/Multi-Language Text", false, 21)]
    static void CreatMultiLanguageText (MenuCommand menuCommand) {
        EditorApplication.ExecuteMenuItem ("GameObject/UI/Text");
        GameObject go = Selection.activeGameObject;
        GameObjectUtility.SetParentAndAlign (go, menuCommand.context as GameObject);
        go.GetComponent<Text> ().raycastTarget = false;
        go.AddComponent<MultiLanguageTextHandler> ();
    }
}