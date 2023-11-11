using System;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Attribute that indicates that an action or perception belongs to a specific group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SelectionGroupAttribute : Attribute
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        public string name;

        /// <summary>
        /// Create a new SelectionGroup attribute.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        public SelectionGroupAttribute(string name)
        {
            this.name = name;
        }
    }
}
