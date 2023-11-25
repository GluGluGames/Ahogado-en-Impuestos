using UnityEditor;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using UtilitySystems;
    [CustomPropertyDrawer(typeof(CurvePoint))]
    public class FunctionPointPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var xRect = new Rect(position.x, position.y, position.width * 0.5f - 5, position.height);
            var yRect = new Rect(position.x + position.width * 0.5f, position.y, position.width * 0.5f - 5, position.height);
            EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"), GUIContent.none);
            EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"), GUIContent.none);
        }
    }
}
