using System;

namespace BehaviourAPI.Core
{
    using Actions;
    /// <summary>
    /// Exception that is thrown when a subsystemAction has no subsystem attached.
    /// </summary>
    public class MissingSubsystemException : NullReferenceException
    {
        /// <summary>
        /// The <see cref="SubsystemAction"/> that throws the exception.
        /// </summary>
        public SubsystemAction SubsystemAction;

        /// <summary>
        /// Create a new <see cref="MissingSubsystemException"/> with the specified <see cref="SubsystemAction"/>.
        /// </summary>
        /// <param name="node">The node that throws the exception.</param>
        public MissingSubsystemException(SubsystemAction subsystemAction)
        {
            SubsystemAction = subsystemAction;
        }

        /// <summary>
        /// Create a new <see cref="MissingSubsystemException"/> with the specified <see cref="SubsystemAction"/> and a message.
        /// </summary>
        /// <param name="SubsystemAction">The node that throws the exception.</param>
        /// <param name="message">The message.</param>
        public MissingSubsystemException(SubsystemAction subsystemAction, string message) : base(message)
        {
            SubsystemAction = subsystemAction;
        }
    }
}
