using System;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    /// <summary>
    /// Attribute to assign a <see cref="NodeDrawer"/> to a node class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomNodeDrawerAttribute : Attribute
    {
        /// <summary>
        /// The type of Node that the annotated drawer renders.
        /// </summary>
        public Type NodeType { get; private set; }

        /// <summary>
        /// Annotates a <see cref="NodeDrawer"/> class to indicates which node type renders.
        /// </summary>
        /// <param name="nodeType">The type of Node</param>
        public CustomNodeDrawerAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }
}
