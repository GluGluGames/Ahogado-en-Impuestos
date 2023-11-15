using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// A element used to modify the execution flow of elements that 
    /// implements the <see cref="IPushActivable"/> interface.
    /// </summary>
    public class PushPerception
    {
        /// <summary>
        /// The list of the elements that will be notified by this push perception.
        /// </summary>
        public List<IPushActivable> PushListeners;

        /// <summary>
        /// Creates a new push perception.
        /// </summary>
        /// <param name="listeners">A list of elements that this push perception will trigger.</param>

        public PushPerception(params IPushActivable[] listeners)
        {
            PushListeners = listeners.ToList();
        }

        /// <summary>
        /// Creates a new push perceptions
        /// </summary>
        /// <param name="listeners">A list of elements that this push perception will trigger.</param>
        public PushPerception(List<IPushActivable> listeners)
        {
            PushListeners = listeners.ToList();
        }

        /// <summary>
        /// Trigger all the elements in the <see cref="PushListeners"/> list.
        /// </summary>
        /// <param name="status">The status used to trigger the element execution.</param>
        public void Fire(Status status = Status.Success) => PushListeners.ForEach(p => p?.Fire(status));
    }
}
