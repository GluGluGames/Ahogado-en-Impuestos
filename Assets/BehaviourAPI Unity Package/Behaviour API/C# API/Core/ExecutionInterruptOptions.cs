namespace BehaviourAPI.Core
{
    /// <summary>
    /// Defines what to do when a element execution is interrupted (try to stop without finish first).
    /// </summary>
    public enum ExecutionInterruptOptions
    {
        /// <summary>
        /// Dont call any event
        /// </summary>
        None = 0,

        /// <summary>
        /// Call pause event
        /// </summary>
        Pause = 1,

        /// <summary>
        /// Call stop event
        /// </summary>
        Stop = 2
    }
}