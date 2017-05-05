using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class TextureAtlasSlicer : EditorWindow {
	[MenuItem("CONTEXT/TextureImporter/Slice Sprite Using XML")]
	public static void SliceUsingXML(MenuCommand command) {
		TextureImporter textureImporter = command.context as TextureImporter;
		TextureAtlasSlicer window = ScriptableObject.CreateInstance<TextureAtlasSlicer>();
		window.importer = textureImporter;
		window.ShowUtility();
	}

	[MenuItem("CONTEXT/TextureImporter/Slice Sprite Using XML", true)]
	public static bool ValidateSliceUsingXML(MenuCommand command) {
		TextureImporter textureImporter = command.context as TextureImporter;
		// valid only if the texture type is 'sprite' or 'advanced'.
		return textureImporter && textureImporter.textureType == TextureImporterType.Sprite ||
			textureImporter.textureType == TextureImporterType.Default;
	}

	public TextureImporter importer;
	public TextureAtlasSlicer() {
        titleContent.text = "Texture Atlas Slicer";
	}

	[SerializeField] private TextAsset xmlAsset;
	public SpriteAlignment spriteAlignment = SpriteAlignment.Center;
	public Vector2 customOffset = new Vector2(0.5f, 0.5f);

	public void OnGUI() {
		xmlAsset = EditorGUILayout.ObjectField("XML Source", xmlAsset, typeof (TextAsset), false) as TextAsset;
		spriteAlignment = (SpriteAlignment) EditorGUILayout.EnumPopup("Pivot", spriteAlignment);

		bool enabled = GUI.enabled;
		if (spriteAlignment != SpriteAlignment.Custom) {
			GUI.enabled = false;
		}
		EditorGUILayout.Vector2Field("Custom Offset", customOffset);
		GUI.enabled = enabled;

		if (xmlAsset == null) {
			GUI.enabled = false;
		}
		if (GUILayout.Button("Slice")) {
			PerformSlice();
        }
		GUI.enabled = enabled;
	}

	private void PerformSlice() {
		XmlDocument document = new XmlDocument();
		document.LoadXml(xmlAsset.text);

		XmlElement root = document.DocumentElement;
        if (root.Name == "TextureAtlas") {
			bool failed = false;

			Texture2D texture = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture2D;
			int textureHeight = texture.height;

			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

			foreach (XmlNode childNode in root.ChildNodes)
			{
				if (childNode.Name == "SubTexture" || childNode.Name == "sprite") {
					int width = Convert.ToInt32(childNode.Attributes["w"].Value);
					// int width = Convert.ToInt32(childNode.Attributes["width"].Value);
					int height = Convert.ToInt32(childNode.Attributes["h"].Value);
					// int height = Convert.ToInt32(childNode.Attributes["height"].Value);
					int x = Convert.ToInt32(childNode.Attributes["x"].Value);
					int y = textureHeight - (height + Convert.ToInt32(childNode.Attributes["y"].Value));

					SpriteMetaData spriteMetaData = new SpriteMetaData {
						alignment = (int)spriteAlignment,
						border = new Vector4(),
						name = childNode.Attributes["n"].Value,
						// name = childNode.Attributes["name"].Value,
						pivot = GetPivotValue(spriteAlignment, customOffset),
						rect = new Rect(x, y, width, height)
					};
					metaDataList.Add(spriteMetaData);
				} else {
					Debug.LogError("Child nodes should be named 'SubTexture' !");
					failed = true;
				}
			}

			if (!failed) {
				importer.spriteImportMode = SpriteImportMode.Multiple; 
				importer.spritesheet = metaDataList.ToArray();
				EditorUtility.SetDirty(importer);
				try {
					AssetDatabase.StartAssetEditing();
					AssetDatabase.ImportAsset(importer.assetPath);
				} finally {
					AssetDatabase.StopAssetEditing();
					Close();
				}
			}
		} else {
			Debug.LogError("XML needs to have a 'TextureAtlas' root node!");
		}
	}

	// SpriteEditorUtility
	public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset) {
		switch (alignment)
		{
		case SpriteAlignment.Center:
			return new Vector2(0.5f, 0.5f);
		case SpriteAlignment.TopLeft:
			return new Vector2(0.0f, 1f);
		case SpriteAlignment.TopCenter:
			return new Vector2(0.5f, 1f);
		case SpriteAlignment.TopRight:
			return new Vector2(1f, 1f);
		case SpriteAlignment.LeftCenter:
			return new Vector2(0.0f, 0.5f);
		case SpriteAlignment.RightCenter:
			return new Vector2(1f, 0.5f);
		case SpriteAlignment.BottomLeft:
			return new Vector2(0.0f, 0.0f);
		case SpriteAlignment.BottomCenter:
			return new Vector2(0.5f, 0.0f);
		case SpriteAlignment.BottomRight:
			return new Vector2(1f, 0.0f);
		case SpriteAlignment.Custom:
			return customOffset;
		default:
			return Vector2.zero;
		}
	}
}