namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Perception used to check the IStatusHandler's execution status.
    /// </summary>
    public class ExecutionStatusPerception : Perception
    {
        /// <summary>
        /// The status handler checked
        /// </summary>
        public IStatusHandler StatusHandler;

        /// <summary>
        /// The status flags that trigger the perception
        /// </summary>
        public StatusFlags StatusFlags;

        /// <summary>
        /// Create a new execution status perception.
        /// </summary>
        public ExecutionStatusPerception()
        {
            StatusFlags = StatusFlags.Running;
        }

        /// <summary>
        /// Create a execution perception that checks if a element's status matches the given flags.
        /// </summary>
        /// <param name="statusHandler">The element checked</param>
        public ExecutionStatusPerception(IStatusHandler statusHandler, StatusFlags flags = StatusFlags.Running)
        {
            StatusHandler = statusHandler;
            StatusFlags = flags;
        }

        /// <summary>
        /// Returns true if the <see cref="StatusHandler"/> current status value matches with <see cref="StatusFlags"/>.
        /// </summary>
        /// <returns></returns>
        public override bool Check()
        {
            if (StatusHandler == null) return false;

            StatusFlags handlerStatusFlag = (StatusFlags)StatusHandler.Status;
            return (handlerStatusFlag & StatusFlags) != 0;
        }
    }
}
