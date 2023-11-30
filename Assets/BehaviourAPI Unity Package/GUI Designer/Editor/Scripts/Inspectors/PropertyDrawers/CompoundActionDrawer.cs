using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Core.Actions;
    using Framework;
    using System.Linq;

    [CustomPropertyDrawer(typeof(CompoundActionWrapper))]
    public class CompoundActionDrawer : PropertyDrawer
    {
        private static readonly float k_RemoveGraphBtnWidth = 40;
        private static readonly float k_SpaceWidth = 10;
        Vector2 _scrollPos;

        private void AddSubAction(SerializedProperty arrayProperty, System.Type perceptionType)
        {
            arrayProperty.arraySize++;
            var lastElementProperty = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).FindPropertyRelative("action");

            if (perceptionType.IsSubclassOf(typeof(CompoundAction)))
            {
                var compound = (CompoundAction)System.Activator.CreateInstance(perceptionType);
                lastElementProperty.managedReferenceValue = new CompoundActionWrapper(compound);
            }
            else
            {
                lastElementProperty.managedReferenceValue = (Action)System.Activator.CreateInstance(perceptionType);
            }

            arrayProperty.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue == null) return;

            var compoundActionProperty = property.FindPropertyRelative("compoundAction");

            var subActionProperty = property.FindPropertyRelative("subActions");

            var labelRect = new Rect(position.x, position.y, position.width - (k_RemoveGraphBtnWidth + k_SpaceWidth), position.height);
            var removeRect = new Rect(position.x + position.width - k_RemoveGraphBtnWidth, position.y, k_RemoveGraphBtnWidth, position.height);
            EditorGUI.LabelField(labelRect, compoundActionProperty.managedReferenceValue.TypeName());

            if (GUI.Button(removeRect, "X"))
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                return;
            }

            int deep = compoundActionProperty.propertyPath.Count(c => c == '.');
            foreach (SerializedProperty p in compoundActionProperty)
            {
                if (p.propertyPath.Count(c => c == '.') == deep + 1)
                {
                    EditorGUILayout.PropertyField(p, true);
                }
            }

            if (GUILayout.Button("Add element", EditorStyles.popup))
            {
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    ElementCreatorWindowProvider.Create<ActionCreationWindow>((pType) => AddSubAction(subActionProperty, pType)));
            }

            GUIStyle centeredLabelstyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

            EditorGUILayout.LabelField("Sub actions", centeredLabelstyle);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "window", GUILayout.MinHeight(300));

            if (subActionProperty != null)
            {
                for (int i = 0; i < subActionProperty.arraySize; i++)
                {
                    var subperception = subActionProperty.GetArrayElementAtIndex(i);
                    var p = subperception.FindPropertyRelative("action");

                    EditorGUILayout.PropertyField(p);
                    if (GUILayout.Button("Remove"))
                    {
                        subActionProperty.DeleteArrayElementAtIndex(i);
                        property.serializedObject.ApplyModifiedProperties();
                        break;
                    }
                    EditorGUILayout.Space(5);
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
