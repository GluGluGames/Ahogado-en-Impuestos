#if UNITY_EDITOR

using UnityEngine;

namespace Plugins.Baedrick.ColoredHeaderCreator
{
	public enum FontStyleOptions
	{
		Bold = 0,
		Normal = 1,
		Italic = 2,
		BoldAndItalic = 3
	}

	public enum TextAlignmentOptions
	{
		Center = 0,
		Left = 1,
		Right = 2
	}

	[System.Serializable]
	public class HeaderSettings
	{
		[Tooltip("Display text for the Header.")]
		public string headerText = "New Header";
		[Tooltip("Header background color.")]
		public Color headerColor = Color.gray;

		[Space(15)]

		[Tooltip("Header text alignment.")]
		public TextAlignmentOptions textAlignmentOptions = TextAlignmentOptions.Center;
		[Tooltip("Header text style.")]
		public FontStyleOptions fontStyleOptions = FontStyleOptions.Bold;
		[Tooltip("Header text size.")]
		public float fontSize = 12.0f;
		[Tooltip("Header text color.")]
		public Color fontColor = Color.white;
		[Tooltip("Header text drop shadow. Warning it is slow.")]
		public bool dropShadow = false;
	}

	public class ColoredHeaderSettings : ScriptableObject
	{
		public HeaderSettings headerSettings = new HeaderSettings();

		public void ResetSettings()
		{
			headerSettings.headerText = "New Header";
			headerSettings.headerColor = Color.gray;
			headerSettings.textAlignmentOptions = TextAlignmentOptions.Center;
			headerSettings.fontStyleOptions = FontStyleOptions.Bold;
			headerSettings.fontSize = 12.0f;
			headerSettings.fontColor = Color.white;
			headerSettings.dropShadow = false;
		}
	}
}

#endif