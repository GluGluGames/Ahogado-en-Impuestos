using UnityEngine;
using UnityEditor;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;

    [CustomPropertyDrawer(typeof(SerializedContextMethod), true)]
    public class SerializedMethodPropertyDrawer : PropertyDrawer
    {
        static float dotWidth = 6f;
        static float space = 3f;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 1f + space;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidth = 60f;
            var fieldWidth = (position.width - dotWidth - labelWidth) * .5f;
            var lineHeight = EditorGUIUtility.singleLineHeight;

            var labelRect = new Rect(position.x, position.y, labelWidth, lineHeight);
            var componentRect = new Rect(position.x + labelWidth, position.y, fieldWidth, lineHeight);
            var separatorRect = new Rect(position.x + labelWidth + fieldWidth, position.y, dotWidth, lineHeight);
            var methodRect = new Rect(position.x + labelWidth + fieldWidth + dotWidth, position.y, fieldWidth, lineHeight);

            EditorGUI.LabelField(labelRect, property.displayName, EditorStyles.miniBoldLabel);
            EditorGUI.PropertyField(componentRect, property.FindPropertyRelative("componentName"), GUIContent.none);
            EditorGUI.LabelField(separatorRect, ".");
            EditorGUI.PropertyField(methodRect, property.FindPropertyRelative("methodName"), GUIContent.none);
        }
    }
}
