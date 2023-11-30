using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Core.Actions;
    using Core.Perceptions;
    using Framework;

    [CustomPropertyDrawer(typeof(ReferenceData))]
    public class ReferenceDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");

            if(valueProperty.managedReferenceValue != null) 
            {
                EditorGUI.PropertyField(position, valueProperty, true);
            }
            else
            {
                var typeProperty = property.FindPropertyRelative("fieldType");
                var type = System.Type.GetType(typeProperty.stringValue);

                if (type == null) return;

                if (typeof(Action).IsAssignableFrom(type))
                {
                    if (GUI.Button(position, new GUIContent("Assign action")))
                    {
                        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                            ElementCreatorWindowProvider.Create<ActionCreationWindow>((aType) => AssignAction(valueProperty, aType), type));
                    }
                }
                else if (typeof(Perception).IsAssignableFrom(type))
                {
                    if (GUI.Button(position, new GUIContent("Assign perception")))
                    {
                        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                            ElementCreatorWindowProvider.Create<PerceptionCreationWindow>((aType) => AssignPerception(valueProperty, aType), type));
                    }
                }
            }

        }

        private void AssignPerception(SerializedProperty property, System.Type pType)
        {
            if (pType.IsSubclassOf(typeof(CompoundPerception)))
            {
                var compound = (CompoundPerception)System.Activator.CreateInstance(pType);
                property.managedReferenceValue = new CompoundPerceptionWrapper(compound);
            }
            else
            {
                property.managedReferenceValue = (Perception)System.Activator.CreateInstance(pType);
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        private void AssignAction(SerializedProperty property, System.Type aType)
        {
            if (aType.IsSubclassOf(typeof(CompoundAction)))
            {
                var compound = (CompoundAction)System.Activator.CreateInstance(aType);
                property.managedReferenceValue = new CompoundActionWrapper(compound);
            }
            else
            {
                property.managedReferenceValue = (Action)System.Activator.CreateInstance(aType);
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
