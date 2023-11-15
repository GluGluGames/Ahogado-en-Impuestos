using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Graphs;
    using System.Linq;
    using UnityEditor;

    public static class VisualElementExtensions
    {
        public static void Disable(this VisualElement visualElement) => visualElement.style.display = DisplayStyle.None;
        public static void Enable(this VisualElement visualElement) => visualElement.style.display = DisplayStyle.Flex;
        public static void Hide(this VisualElement visualElement) => visualElement.style.visibility = Visibility.Hidden;
        public static void Show(this VisualElement visualElement) => visualElement.style.visibility = Visibility.Visible;

        public static bool IsHorizontal(this EPortOrientation portOrientation) => portOrientation == EPortOrientation.Left || portOrientation == EPortOrientation.Right;

        public static Orientation ToOrientation(this EPortOrientation portOrientation)
        {
            if (portOrientation.IsHorizontal()) return Orientation.Horizontal;
            else return Orientation.Vertical;
        }

        public static Vector2 ToVector(this EPortOrientation portOrientation)
        {
            if (portOrientation == EPortOrientation.Top) return Vector2.up;
            if (portOrientation == EPortOrientation.Bottom) return Vector2.down;
            if (portOrientation == EPortOrientation.Left) return Vector2.left;
            if (portOrientation == EPortOrientation.Right) return Vector2.right;
            else return Vector2.zero;
        }

        public static FlexDirection ToFlexDirection(this EPortOrientation portOrientation)
        {
            if (portOrientation == EPortOrientation.Top) return FlexDirection.ColumnReverse;
            if (portOrientation == EPortOrientation.Bottom) return FlexDirection.Column;
            if (portOrientation == EPortOrientation.Left) return FlexDirection.Row;
            if (portOrientation == EPortOrientation.Right) return FlexDirection.RowReverse;
            else return FlexDirection.Column;
        }

        public static void ChangeBorderColor(this VisualElement visualElement, Color color)
        {
            visualElement.style.borderBottomColor = color;
            visualElement.style.borderTopColor = color;
            visualElement.style.borderLeftColor = color;
            visualElement.style.borderRightColor = color;
        }

        public static void ChangeBackgroundColor(this VisualElement visualElement, Color color)
        {
            visualElement.style.backgroundColor = color;
        }


        public static void AddGroup(this List<SearchTreeEntry> searchTreeEntries, string title, int level)
        {
            searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(title), level));
        }

        public static void AddEntry(this List<SearchTreeEntry> searchTreeEntries, string title, int level, object userData)
        {
            searchTreeEntries.Add(new SearchTreeEntry(new GUIContent("     " + title)) { level = level, userData = userData });
        }

        public static DropdownMenuAction.Status ToMenuStatus(this bool b) => b ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;


        public static void Align(this Port port, EPortOrientation orientation, Direction dir)
        {
            var middleValue = new StyleLength(new Length(50, LengthUnit.Percent));
            if (orientation == EPortOrientation.None) return;

            port.style.position = Position.Absolute;
            if (orientation == EPortOrientation.Top)
            {
                port.style.top = 0;
                if (dir == Direction.Input) port.style.right = middleValue;
                else port.style.left = middleValue;
            }
            else if (orientation == EPortOrientation.Right)
            {
                port.style.right = 0;
                if (dir == Direction.Input) port.style.bottom = middleValue;
                else port.style.top = middleValue;
            }
            else if (orientation == EPortOrientation.Bottom)
            {
                port.style.bottom = 0;
                if (dir == Direction.Input) port.style.left = middleValue;
                else port.style.right = middleValue;
            }
            else
            {
                port.style.left = 0;
                if (dir == Direction.Input) port.style.top = middleValue;
                else port.style.bottom = middleValue;
            }
        }

        public static IEnumerable<SerializedProperty> GetChildProperties(this SerializedProperty property)
        {
            int deep = property.propertyPath.Count(c => c == '.');
            foreach (SerializedProperty p in property)
            {
                if (p.propertyPath.Count(c => c == '.') == deep + 1)
                {
                    yield return p.Copy();
                }
            }
        }
    }
}
