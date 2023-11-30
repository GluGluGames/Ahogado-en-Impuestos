using System;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    /// <summary>
    /// Attribute to assign a <see cref="GraphAdapter"/> to a BehaviourGraph class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomGraphAdapterAttribute : Attribute
    {
        /// <summary>
        /// The type of BehaviourGraph that the annotated adapter manages.
        /// </summary>
        public Type GraphType;

        /// <summary>
        /// Annotates a <see cref="GraphAdapter"/> class to indicates which behaviour graph type manages.
        /// </summary>
        /// <param name="type">The type of BehaviourGraph.</param>
        public CustomGraphAdapterAttribute(Type type)
        {
            this.GraphType = type;
        }
    }
}
