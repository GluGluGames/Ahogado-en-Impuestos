using System;

namespace BehaviourAPI.Core
{
    /// <summary> 
    /// Interface for elements that handles an execution status. 
    /// </summary>
    public interface IStatusHandler
    {
        /// <summary> 
        /// Event invoked when Status value changed. 
        /// </summary>
        /// <value> The status changed event. </value>

        Action<Status> StatusChanged { get; set; }

        /// <summary> 
        /// Gets the status of the element. 
        /// </summary>
        /// <value> The execution status. </value>

        Status Status { get; }
    }

}