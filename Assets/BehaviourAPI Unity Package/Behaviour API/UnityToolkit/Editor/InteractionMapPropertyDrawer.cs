using BehaviourAPI.UnityToolkit;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InteractionMap))]
public class InteractionMapPropertyDrawer : PropertyDrawer
{
    private string currentKey;

    private static readonly int k_DeletebtnSize = 30;
    private static readonly int k_ElemSpaceSize = 10;
    private static readonly int k_RowSpaceSize = 10;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty keyProp = property.FindPropertyRelative("keys");
        SerializedProperty valueProp = property.FindPropertyRelative("values");

        int count = Mathf.Max(keyProp.arraySize, valueProp.arraySize);
        float delta = EditorGUIUtility.singleLineHeight;

        Rect createRect = new Rect(position.x, position.y, position.width, delta);
        currentKey = EditorGUI.TextField(createRect, currentKey);

        Rect btnRect = new Rect(position.x, position.y + delta, position.width, delta);
        if (GUI.Button(btnRect, "Add interaction") && !string.IsNullOrEmpty(currentKey))
        {
            keyProp.InsertArrayElementAtIndex(keyProp.arraySize);
            keyProp.GetArrayElementAtIndex(keyProp.arraySize - 1).stringValue = currentKey;
            valueProp.InsertArrayElementAtIndex(valueProp.arraySize);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            var vProp = valueProp.GetArrayElementAtIndex(i);
            var kProp = keyProp.GetArrayElementAtIndex(i);

            Rect sliderRect = new Rect(position.x, position.y + delta * (i + 2), position.width - (k_ElemSpaceSize + k_DeletebtnSize), delta);
            Rect deleteRect = new Rect(position.x + position.width - k_DeletebtnSize, position.y + delta * (i + 2), k_DeletebtnSize, delta);

            EditorGUI.PropertyField(sliderRect, vProp, new GUIContent(kProp.stringValue));

            if (GUI.Button(deleteRect, "X"))
            {
                keyProp.DeleteArrayElementAtIndex(i);
                valueProp.DeleteArrayElementAtIndex(i);
                break;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keyProp = property.FindPropertyRelative("keys");
        SerializedProperty valueProp = property.FindPropertyRelative("values");
        int count = Mathf.Max(keyProp.arraySize, valueProp.arraySize);

        return EditorGUIUtility.singleLineHeight * (count + 2) + k_RowSpaceSize * (count);
    }
}
