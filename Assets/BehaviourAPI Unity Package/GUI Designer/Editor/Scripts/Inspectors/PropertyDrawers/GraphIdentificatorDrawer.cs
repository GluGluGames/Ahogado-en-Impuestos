using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;

    [CustomPropertyDrawer(typeof(GraphIdentificatorAttribute))]
    public class GraphIdentificatorDrawer : PropertyDrawer
    {
        private static readonly float k_RemoveGraphBtnWidth = 40;
        private static readonly float k_SpaceWidth = 10;

        private void SetSubgraph(SerializedProperty property, GraphData data)
        {
            property.stringValue = data.id;
            property.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String) return;

            if (string.IsNullOrEmpty(property.stringValue))
            {
                if (GUI.Button(position, "Assign subgraph"))
                {
                    var provider = ElementSearchWindowProvider<GraphData>.Create<GraphSearchWindowProvider>((g) => SetSubgraph(property, g));
                    provider.Data = BehaviourSystemEditorWindow.instance.System.Data;
                    SearchWindow.Open(new SearchWindowContext(Event.current.mousePosition + BehaviourSystemEditorWindow.instance.position.position), provider);
                }
            }
            else
            {
                Rect labelRect = new Rect(position.x, position.y, position.width - (k_RemoveGraphBtnWidth + k_SpaceWidth), position.height);
                Rect btnRect = new Rect(position.x + position.width - k_RemoveGraphBtnWidth, position.y, k_RemoveGraphBtnWidth, position.height);

                var subgraph = BehaviourSystemEditorWindow.instance.System.Data.graphs.Find(g => g.id == property.stringValue);
                EditorGUI.LabelField(labelRect, "SUBGRAPH: " + subgraph?.name ?? "missing subgraph");
                if (GUI.Button(btnRect, "X"))
                {
                    property.stringValue = string.Empty;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
