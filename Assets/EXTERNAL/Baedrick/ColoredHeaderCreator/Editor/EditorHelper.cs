using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Plugins.Baedrick.ColoredHeaderCreator.Editor
{
	public static class EditorHelper
	{
		public static bool Foldout(bool foldout, string content)
		{
			var rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

			var foldoutTintColor = EditorGUIUtility.isProSkin
				? new Color(1f, 1f, 1f, 0.05f)
				: new Color(0f, 0f, 0f, 0.05f);
			EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), foldoutTintColor);

			var foldoutRect = rect;
			foldoutRect.width = EditorGUIUtility.singleLineHeight;
			foldout = EditorGUI.Foldout(rect, foldout, "", true);

			rect.x += EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);

			return foldout;
		}

		public static ColoredHeaderSettings LoadSettings(string path)
		{
			var settingsObject = (ColoredHeaderSettings)EditorGUIUtility.Load(path);
			if (settingsObject != null) {
				return settingsObject;
			}

			var guids = AssetDatabase.FindAssets("t:" + nameof(ColoredHeaderSettings));
			if (guids.Length > 0) {
				return AssetDatabase.LoadAssetAtPath<ColoredHeaderSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
			}

			return null;
		}

		public static void CreateSettingsAsset(string path)
		{
			var settingsObject = ScriptableObject.CreateInstance<ColoredHeaderSettings>();
			AssetDatabase.CreateAsset(settingsObject, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void CopyHeaderSettingsFromTo(HeaderSettings from, HeaderSettings to)
		{
			to.headerText = from.headerText;
			to.headerColor = from.headerColor;
			to.textAlignmentOptions = from.textAlignmentOptions;
			to.fontStyleOptions = from.fontStyleOptions;
			to.fontSize = from.fontSize;
			to.fontColor = from.fontColor;
			to.dropShadow = from.dropShadow;
		}
		
		public static void DeleteAllHeaders()
		{
			var headerComponents = Object.FindObjectsOfType<ColoredHeader>();
			foreach (var headerComponent in headerComponents) {
				if (headerComponent.transform.parent == null) {
					headerComponent.transform.DetachChildren();
					Undo.DestroyObjectImmediate(headerComponent.gameObject);
					continue;
				}

				if (headerComponent.transform.parent != null && headerComponent.transform.childCount > 0) {
					var parent = headerComponent.transform.parent;
					var children = new List<Transform>();
					for (var i = 0; i < headerComponent.transform.childCount; ++i) {
						children.Add(headerComponent.transform.GetChild(i));
					}

					headerComponent.transform.DetachChildren();
					foreach (var child in children) {
						child.parent = parent;
					}
					Undo.DestroyObjectImmediate(headerComponent.gameObject);
					continue;
				}
				Undo.DestroyObjectImmediate(headerComponent.gameObject);
			}
		}
	}
}