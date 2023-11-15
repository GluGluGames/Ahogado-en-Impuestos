using System.Collections.Generic;

namespace BehaviourAPI.SmartObjects
{
    using BehaviourAPI.Core;
    using Core.Actions;

    /// <summary>   
    /// Represents an interaction between a smart agent and a smart object. 
    /// When the interaction is completed, a agent's need is covered.
    /// </summary>
    public class SmartInteraction
    {
        /// <summary> 
        /// The action that this interaction executes. </summary>
        /// <value> The interaction action. </value>
        public Action Action { get; private set; }

        /// <summary>
        /// Event called when the interaction is started.
        /// </summary>
        public event System.Action OnInitialize;

        /// <summary>
        /// Event called when the interaction is completed.
        /// </summary>
        public event System.Action<Status> OnComplete;

        /// <summary>
        /// Event called when the interaction is stopped or interrupted.
        /// </summary>
        public event System.Action OnRelease;

        /// <summary>
        /// Event called when the interaction is paused.
        /// </summary>
        public event System.Action OnPause;

        /// <summary>
        /// Event called when the interaction is unpaused.
        /// </summary>
        public event System.Action OnUnpause;

        /// <summary>
        /// The capabilities that will be applied to the agent if the interaction is completed successfully.
        /// </summary>
        public Dictionary<string, float> Capabilities { get; private set; }

        /// <summary>
        /// The smart agent that was used to request the interaction.
        /// </summary>
        public ISmartAgent Agent { get; private set; }

        /// <summary> 
        /// Constructor. 
        /// </summary>
        /// <param name="agent"> The smart agent that was used to request the interaction. </param>
        /// <param name="action"> The action that this interaction executes </param>
        /// <param name="capabilities"> The capabilities that will be applied to the agent if the interaction is completed successfully. </param>
        public SmartInteraction(Action action, ISmartAgent agent, Dictionary<string, float> capabilities)
        {
            Action = action;
            Capabilities = capabilities;
            Agent = agent;
        }

        /// <summary>
        /// Initialize the interaction.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public void Initialize(ExecutionContext context)
        {
            OnInitialize?.Invoke();
            if (context != null) Action.SetExecutionContext(context);
            Action.Start();
        }

        /// <summary>
        /// Update the interaction.
        /// </summary>
        /// <returns>The status returned by the action.</returns>
        public Status Update()
        {
            var status = Action.Update();

            if (status == Status.Success)
            {
                foreach (var capability in Capabilities)
                {
                    Agent.CoverNeed(capability.Key, capability.Value);
                }
            }

            if (status != Status.Running) OnComplete?.Invoke(status);
            return status;
        }

        /// <summary>
        /// Stops the execution of the interaction, even without finish.
        /// </summary>
        public void Release()
        {
            Action.Stop();
            OnRelease?.Invoke();
        }

        /// <summary>
        /// Pause the interaction
        /// </summary>
        public void Pause()
        {
            Action.Pause();
            OnPause?.Invoke();
        }

        /// <summary>
        /// Unpause the interaction
        /// </summary>
        public void Unpause()
        {
            Action.Unpause();
            OnUnpause?.Invoke();
        }
    }
}
