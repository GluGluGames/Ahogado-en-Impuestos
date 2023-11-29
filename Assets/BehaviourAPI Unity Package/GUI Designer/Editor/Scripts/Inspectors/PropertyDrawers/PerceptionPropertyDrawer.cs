using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Core.Perceptions;
    using Framework;

    [CustomPropertyDrawer(typeof(Perception))]
    public class PerceptionPropertyDrawer : PropertyDrawer
    {
        private static readonly float k_RemoveGraphBtnWidth = 40;
        private static readonly float k_SpaceWidth = 10;

        private void AssignPerception(SerializedProperty property, Type perceptionType)
        {
            if (perceptionType.IsSubclassOf(typeof(CompoundPerception)))
            {
                var compound = (CompoundPerception)Activator.CreateInstance(perceptionType);
                property.managedReferenceValue = new CompoundPerceptionWrapper(compound);
            }
            else
            {
                property.managedReferenceValue = (Perception)Activator.CreateInstance(perceptionType);
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            if (property.managedReferenceValue == null)
            {
                if (GUILayout.Button("Assign perception"))
                {
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), ElementCreatorWindowProvider.Create<PerceptionCreationWindow>((pType) => AssignPerception(property, pType)));
                }
            }
            else
            {
                var labelRect = new Rect(position.x, position.y, position.width - (k_RemoveGraphBtnWidth + k_SpaceWidth), position.height);
                var removeRect = new Rect(position.x + position.width - k_RemoveGraphBtnWidth, position.y, k_RemoveGraphBtnWidth, position.height);
                EditorGUI.LabelField(labelRect, property.managedReferenceValue.TypeName());
                if (GUI.Button(removeRect, "X"))
                {
                    property.managedReferenceValue = null;
                }
                else
                {
                    int deep = property.propertyPath.Count(c => c == '.');
                    foreach (SerializedProperty p in property)
                    {
                        if (p.propertyPath.Count(c => c == '.') == deep + 1)
                        {
                            EditorGUILayout.PropertyField(p, true);
                        }
                    }
                }
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
