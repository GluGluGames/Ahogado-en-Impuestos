namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Subclass of  <see cref="BehaviourRunner"/> that executes a reusable <see cref="BehaviourSystem"/> 
    /// </summary>
    public class AssetBehaviourRunner : DataBehaviourRunner
    {
        public BehaviourSystem System;
        SystemData _runtimeSystem = null;

        public override SystemData Data => _runtimeSystem;

        /// <summary>
        /// Returns the system asset data to generate a runtime copy
        /// </summary>
        protected sealed override SystemData GetEditedSystemData()
        {
            _runtimeSystem = System.GetBehaviourSystemData();
            return _runtimeSystem;
        }
    }
}
