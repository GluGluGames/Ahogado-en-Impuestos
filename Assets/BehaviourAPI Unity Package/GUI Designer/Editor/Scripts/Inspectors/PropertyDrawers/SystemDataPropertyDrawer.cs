using UnityEditor;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;

    [CustomPropertyDrawer(typeof(SystemData))]
    public class SystemDataPropertyDrawer : PropertyDrawer
    {
        float k_TitleHeight = EditorGUIUtility.singleLineHeight;
        float k_Spacer = 5f;
        float k_Separatorheight = 2f;
        float k_numberWidth = 30f;

        Color k_TitleColor = new Color(.25f, .25f, .25f);
        Color k_SeparatorColor = new Color(.25f, .25f, .25f);

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            var lastColor = GUI.contentColor;

            GUI.contentColor = Color.white;
            var h = EditorGUIUtility.singleLineHeight * 1.2f;
            float currentHeight = pos.y + k_Spacer + h;
            var titleRect = new Rect(pos.x, currentHeight, pos.width, k_TitleHeight);
            currentHeight += k_TitleHeight + k_Spacer;

            SerializedProperty graphsProp = property.FindPropertyRelative("graphs");

            float nameWidth = (pos.width - k_numberWidth - k_Spacer * 2f) * 0.6f;
            float typeWidth = (pos.width - k_numberWidth - k_Spacer * 2f) * 0.4f;

            var titleStyle = new GUIStyle(GUI.skin.box);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.background = GetTitleTexture();
            GUI.Label(titleRect, "BEHAVIOUR SYSTEM", titleStyle);
            EditorGUI.DrawRect(new Rect(pos.x, currentHeight, pos.width, k_Separatorheight), k_SeparatorColor);

            currentHeight += k_Spacer * 2f;

            var style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleLeft;

            if(graphsProp.arraySize == 0)
            {
                var rect = new Rect(pos.x, currentHeight, pos.width, h);
                GUI.Label(rect, "No graphs", "box");
                currentHeight += h + k_Spacer;
            }
            else
            {
                var graphsRect = new Rect(pos.x, currentHeight, pos.width - k_numberWidth - k_Spacer, h);
                var ngRect = new Rect(pos.x + (pos.width - k_numberWidth), currentHeight, k_numberWidth, h);

                GUI.Label(graphsRect, "Graphs:", style);
                GUI.Label(ngRect, graphsProp.arraySize.ToString(), "box");
                currentHeight += h + k_Spacer;

                for (int i = 0; i <= graphsProp.arraySize; i++)
                {
                    float currentWidth = pos.x;
                    var nameRect = new Rect(currentWidth, currentHeight, nameWidth, h);
                    currentWidth += nameWidth + k_Spacer;
                    var typeRect = new Rect(currentWidth, currentHeight, typeWidth, h);
                    currentWidth += typeWidth + k_Spacer;
                    var numberRect = new Rect(currentWidth, currentHeight, k_numberWidth, h);
                    currentHeight += h + k_Spacer;

                    if (i == 0)
                    {
                        GUI.Label(nameRect, "NAME", "box");
                        GUI.Label(typeRect, "TYPE", "box");
                        GUI.Label(numberRect, "N", "box");
                    }
                    else
                    {
                        SerializedProperty currentGraph = graphsProp.GetArrayElementAtIndex(i - 1);
                        GUI.Label(nameRect, currentGraph.FindPropertyRelative("name").stringValue, style);
                        GUI.Label(typeRect, currentGraph.FindPropertyRelative("graph").managedReferenceValue.TypeName(), style);
                        GUI.Label(numberRect, currentGraph.FindPropertyRelative("nodes").arraySize.ToString(), "box");
                    }
                }
            }            

            EditorGUI.DrawRect(new Rect(pos.x, currentHeight, pos.width, k_Separatorheight), k_SeparatorColor);
            currentHeight += k_Spacer;

            var pushRect = new Rect(pos.x, currentHeight, pos.width - k_numberWidth - k_Spacer, h);
            var nRect = new Rect(pos.x + (pos.width - k_numberWidth), currentHeight, k_numberWidth, h);

            GUI.Label(pushRect, "Push Perceptions:", style);
            GUI.Label(nRect, property.FindPropertyRelative("pushPerceptions").arraySize.ToString(), "box");
            GUI.contentColor = lastColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int graphCount = property.FindPropertyRelative("graphs").arraySize;

            int lines = graphCount + 5;

            return (EditorGUIUtility.singleLineHeight * 1.2f + k_Spacer) * lines + k_TitleHeight;
        }

        private Texture2D GetTitleTexture()
        {
            Texture2D texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, k_TitleColor);
            texture.Apply();
            return texture;
        }
    }
}
