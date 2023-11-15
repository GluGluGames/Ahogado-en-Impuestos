using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    /// <summary>
    /// Element that is editable in behaviour system editor window.
    /// </summary>
    public interface IBehaviourSystem
    {
        /// <summary>
        /// The data that stores all the behaviour system elements.
        /// </summary>
        public SystemData Data { get; }

        /// <summary>
        /// Reference to the unity object that serializes the data.
        /// </summary>
        public Object ObjectReference { get; }       
    }
}