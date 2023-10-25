using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Plugins.Baedrick.ColoredHeaderCreator.Editor
{
	public class ColoredHeaderGUI : EditorWindow
	{
		private static ColoredHeaderSettings settings;
		private const string PATH_TO_SETTINGS = "Assets/Plugins/Baedrick/ColoredHeaderCreator/Settings.asset";

		private static Object headerPresetToLoad;

		private enum Tabs
		{
			HeaderCreator = 0,
			PresetCreator = 1
		}

		private static int selectedTab = 0;
		private static bool headerBoxFoldout = true;
		private static bool headerFontFoldout = true;
		private static bool loadPresetFoldout = true;
		private static bool createPresetFoldout = true;

		private static class Strings
		{
			public const string HEADER_OBJECT_NAME = "%$ Header";
			public const string WINDOW_TITLE = "Colored Headers";
			public const string LOGO_TEXT = "Baedrick | ColoredHeaders";

			public static readonly string[] TAB_NAMES = { "Header Creator", "Header Presets" };

			public const string HEADER_SETTINGS_FOLDOUT_LABEL = "Header Settings";
			public const string FONT_SETTINGS_FOLDOUT_LABEL = "Font Settings";
			public const string LOAD_HEADER_FOLDOUT_LABEL = "Load Header Preset";
			public const string CREATE_HEADER_FOLDOUT_LABEL = "Create Header Preset";

			public const string HEADER_TITLE_LABEL = "Header Name";
			public const string HEADER_TITLE_TOOLTIP = "Display text for the Header.";
			public const string HEADER_COLOR_LABEL = "Header Color";
			public const string HEADER_COLOR_TOOLTIP = "Header background color.";

			public const string TEXT_ALIGNMENT_LABEL = "Text Alignment";
			public const string TEXT_ALIGNMENT_TOOLTIP = "Header text alignment.";
			public const string FONT_STYLE_LABEL = "Font Style";
			public const string FONT_STYLE_TOOLTIP = "Header text font style.";
			public const string FONT_SIZE_LABEL = "Font Size";
			public const string FONT_SIZE_TOOLTIP = "Header text font size.";
			public const string FONT_COLOR_LABEL = "Font Color";
			public const string FONT_COLOR_TOOLTIP = "Header text font color.";
			public const string DROP_SHADOW_LABEL = "Drop Shadow (Slow)";
			public const string DROP_SHADOW_TOOLTIP = "Header text drop shadow. Warning: it is slow.";

			public const string HEADER_PRESET_INPUT_LABEL = "Header Preset File";
			public const string HEADER_PRESET_INPUT_TOOLTIP = "Header Preset File to load.";

			public const string CREATE_HEADER_BUTTON_LABEL = "Create Header";
			public const string RESET_BUTTON_LABEL = "Reset To Default";
			public const string DELETE_HEADER_BUTTON_LABEL = "Delete All Headers";

			public const string LOAD_PRESET_BUTTON_LABEL = "Load Headers From File";
			public const string SAVE_PRESET_BUTTON_LABEL = "Save Scene Headers As Preset";

			public const string SAVE_PRESET_PANEL_TITLE = "Save Colored Header Preset File";
		}

		[MenuItem("GameObject/Colored Header", false, 10)]
		private static void CreateHeaderWithMenu(MenuCommand menuCommand)
		{
			var headerObject = new GameObject
			{
				name = Strings.HEADER_OBJECT_NAME
			};
			GameObjectUtility.SetParentAndAlign(headerObject, menuCommand.context as GameObject);
			headerObject.AddComponent<ColoredHeader>();
			Undo.RegisterCreatedObjectUndo(headerObject, "Create Header");
			EditorApplication.RepaintHierarchyWindow();
		}
		
		[MenuItem("Tools/Colored Header Creator %H")]
		private static void CreateWindow()
		{
			var window = GetWindow<ColoredHeaderGUI>(Strings.WINDOW_TITLE);
			window.minSize = new Vector2(325, 390);
		}

		private void OnEnable()
		{
			if (EditorHelper.LoadSettings(PATH_TO_SETTINGS) == null) {
				EditorHelper.CreateSettingsAsset(PATH_TO_SETTINGS);
			}

			settings = EditorHelper.LoadSettings(PATH_TO_SETTINGS);
		}

		private void OnDisable()
		{
			EditorUtility.SetDirty(settings);
		}

		private void OnGUI()
		{
			var logoRect = EditorGUILayout.GetControlRect(GUILayout.Height(50f));
			var logoFont = new GUIStyle(EditorStyles.label)
			{
				alignment = TextAnchor.MiddleCenter,
				fontSize = 20
			};
			if (Event.current.type == EventType.Repaint) {
				logoFont.Draw(logoRect, Strings.LOGO_TEXT, false, false, false, false);
			}

			Undo.undoRedoPerformed += Repaint;

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(15.0f);
				EditorGUI.BeginChangeCheck();

				selectedTab = GUILayout.Toolbar(selectedTab, Strings.TAB_NAMES, GUILayout.Height(26f));
				
				if (EditorGUI.EndChangeCheck()) {
					GUI.FocusControl(null);
				}
				GUILayout.Space(10.0f);
			}
			GUILayout.EndHorizontal();

			switch ((Tabs)selectedTab) {
				case Tabs.HeaderCreator:
					DrawHeaderCreator();
					break;
				case Tabs.PresetCreator:
					DrawPresetCreator();
					break;
				default:
					throw new System.ArgumentOutOfRangeException();
			}
		}

		private static void DrawHeaderCreator()
		{
			GUILayout.Space(8.0f);

			headerBoxFoldout = EditorHelper.Foldout(headerBoxFoldout, Strings.HEADER_SETTINGS_FOLDOUT_LABEL);
			if (headerBoxFoldout) {
				EditorGUI.BeginChangeCheck();
				var headerSettings = settings.headerSettings;
				var headerTextUndo = headerSettings.headerText;
				var headerColorUndo = headerSettings.headerColor;

				headerTextUndo = EditorGUILayout.TextField(
					new GUIContent(Strings.HEADER_TITLE_LABEL, Strings.HEADER_TITLE_TOOLTIP), headerTextUndo);
				headerColorUndo = EditorGUILayout.ColorField(
					new GUIContent(Strings.HEADER_COLOR_LABEL, Strings.HEADER_COLOR_TOOLTIP), headerColorUndo);

				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(settings, "Undo: Header Box Settings.");
					settings.headerSettings.headerText = headerTextUndo;
					settings.headerSettings.headerColor = headerColorUndo;
				}
			}

			headerFontFoldout = EditorHelper.Foldout(headerFontFoldout, Strings.FONT_SETTINGS_FOLDOUT_LABEL);
			if (headerFontFoldout) {
				EditorGUI.BeginChangeCheck();
				var headerSettings = settings.headerSettings;
				var textAlignmentUndo = headerSettings.textAlignmentOptions;
				var fontStyleUndo = headerSettings.fontStyleOptions;
				var fontSizeUndo = headerSettings.fontSize;
				var fontColorUndo = headerSettings.fontColor;
				var dropShadowUndo = headerSettings.dropShadow;

				textAlignmentUndo = (TextAlignmentOptions)EditorGUILayout.EnumPopup(
					new GUIContent(Strings.TEXT_ALIGNMENT_LABEL, Strings.TEXT_ALIGNMENT_TOOLTIP), textAlignmentUndo);
				fontStyleUndo = (FontStyleOptions)EditorGUILayout.EnumPopup(
					new GUIContent(Strings.FONT_STYLE_LABEL, Strings.FONT_STYLE_TOOLTIP), fontStyleUndo);
				fontSizeUndo = EditorGUILayout.Slider(
					new GUIContent(Strings.FONT_SIZE_LABEL, Strings.FONT_SIZE_TOOLTIP), fontSizeUndo, 1, 20);
				fontColorUndo = EditorGUILayout.ColorField(
					new GUIContent(Strings.FONT_COLOR_LABEL, Strings.FONT_COLOR_TOOLTIP), fontColorUndo);
				dropShadowUndo = EditorGUILayout.Toggle(
					new GUIContent(Strings.DROP_SHADOW_LABEL, Strings.DROP_SHADOW_TOOLTIP), dropShadowUndo);

				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(settings, "Undo: Font Box Settings.");
					settings.headerSettings.textAlignmentOptions = textAlignmentUndo;
					settings.headerSettings.fontStyleOptions = fontStyleUndo;
					settings.headerSettings.fontSize = fontSizeUndo;
					settings.headerSettings.fontColor = fontColorUndo;
					settings.headerSettings.dropShadow = dropShadowUndo;
				}
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button(Strings.CREATE_HEADER_BUTTON_LABEL, GUILayout.MinHeight(50f))) {
				CreateHeader(settings.headerSettings);
			}

			GUILayout.Space(2.0f);

			if (GUILayout.Button(Strings.RESET_BUTTON_LABEL)) {
				Undo.RegisterCompleteObjectUndo(settings, "Undo: Header Creator Settings Field.");
				settings.ResetSettings();
			}

			if (GUILayout.Button(Strings.DELETE_HEADER_BUTTON_LABEL)) {
				EditorHelper.DeleteAllHeaders();
			}
		}

		private static void DrawPresetCreator()
		{
			loadPresetFoldout = EditorHelper.Foldout(loadPresetFoldout, Strings.LOAD_HEADER_FOLDOUT_LABEL);
			if (loadPresetFoldout) {
				headerPresetToLoad = EditorGUILayout.ObjectField(
					new GUIContent(Strings.HEADER_PRESET_INPUT_LABEL, Strings.HEADER_PRESET_INPUT_TOOLTIP),
					headerPresetToLoad, typeof(ColoredHeaderPreset), false);
				if (GUILayout.Button(Strings.LOAD_PRESET_BUTTON_LABEL, GUILayout.MinHeight(30f))) {
					CreateHeadersFromPreset((ColoredHeaderPreset)headerPresetToLoad);
				}
			}

			createPresetFoldout = EditorHelper.Foldout(createPresetFoldout, Strings.CREATE_HEADER_FOLDOUT_LABEL);
			if (createPresetFoldout) {
				if (GUILayout.Button(Strings.SAVE_PRESET_BUTTON_LABEL, GUILayout.MinHeight(30f))) {
					CreatePresetFile();
				}
			}
		}

		private static void CreateHeader(HeaderSettings headerSettings)
		{
			var headerObject = new GameObject
			{
				name = Strings.HEADER_OBJECT_NAME,
				transform =
				{
					position = Vector3.zero,
					rotation = Quaternion.identity,
					localScale = Vector3.one
				}
			};
			var headerComponentSettings = headerObject.AddComponent<ColoredHeader>().headerSettings;
			EditorHelper.CopyHeaderSettingsFromTo(headerSettings, headerComponentSettings);

			Undo.RegisterCreatedObjectUndo(headerObject, "Create Header");
			EditorApplication.RepaintHierarchyWindow();
		}

		private static void CreateHeadersFromPreset(ColoredHeaderPreset presetFile)
		{
			if (presetFile == null) {
				Debug.LogError("Invalid operation.");
				return;
			}

			foreach (var headerSettings in presetFile.coloredHeaderPreset) {
				CreateHeader(headerSettings);
			}
		}

		private static void CreatePresetFile()
		{
			var path = EditorUtility.SaveFilePanelInProject(Strings.SAVE_PRESET_PANEL_TITLE, "New Header Preset", "asset", "Enter a file name.");
			if (path.Length <= 0) {
				Debug.LogError("Invalid operation.");
				return;
			}

			var asset = CreateInstance<ColoredHeaderPreset>();
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();

			var presetAsset = (ColoredHeaderPreset)EditorGUIUtility.Load(path);
			presetAsset.coloredHeaderPreset.Clear();

			var headerComponents = FindObjectsOfType<ColoredHeader>();
			foreach (var headerComponent in headerComponents) {
				var headerSettings = headerComponent.headerSettings;
				var presetSettings = new HeaderSettings();

				EditorHelper.CopyHeaderSettingsFromTo(headerSettings, presetSettings);
				presetAsset.coloredHeaderPreset.Add(presetSettings);
				EditorUtility.SetDirty(presetAsset);
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}
