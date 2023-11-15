using System;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;
    using Core.Perceptions;

    /// <summary>
    /// Adaptation wrapper class for serialize <see cref="ConditionPerception"/> subperception in editor tools. 
    /// <para>! -- Don't use this class directly in code.</para>
    /// </summary>
    [Serializable]
    public class PerceptionWrapper
    {
        /// <summary>
        /// The wrapped perception
        /// </summary>
        [SerializeReference] public Perception perception;

        public PerceptionWrapper(Perception perception)
        {
            this.perception = perception;
        }

        public PerceptionWrapper()
        {
        }
    }
}
