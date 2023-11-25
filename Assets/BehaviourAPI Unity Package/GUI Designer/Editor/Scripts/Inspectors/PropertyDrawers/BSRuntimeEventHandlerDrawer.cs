using UnityEditor;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Runtime;

    [CustomPropertyDrawer(typeof(BSRuntimeEventHandler))]
    public class BSRuntimeEventHandlerDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var h = EditorGUIUtility.singleLineHeight;
            var currentHeight = position.y;

            Rect rect = new Rect(position.x, currentHeight, position.width, h);
            GUI.Label(rect, "DEBUG", EditorStyles.centeredGreyMiniLabel);
            rect.y += h;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("debugStatusChanges"));
            rect.y += h;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("identifier"));
        }
    }
}
