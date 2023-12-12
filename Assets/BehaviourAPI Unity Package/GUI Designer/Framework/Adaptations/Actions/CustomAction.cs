using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core.Actions;

    /// <summary>
    /// Adaptation class for use custom <see cref="FunctionalAction"/> in editor tools.
    /// <para>! -- Don't use this class directly in code.</para>
    /// </summary>
    public class CustomAction : FunctionalAction, IBuildable
    {
        /// <summary>
        /// Method reference for start event.
        /// </summary>
        public ContextualSerializedAction start;

        /// <summary>
        /// Method reference for update event.
        /// </summary>
        public ContextualSerializedStatusFunction update;

        /// <summary>
        /// Method reference for stop event.
        /// </summary>
        public ContextualSerializedAction stop;

        /// <summary>
        /// Method reference for update event.
        /// </summary>
        public ContextualSerializedAction pause;

        /// <summary>
        /// Method reference for stop event.
        /// </summary>
        public ContextualSerializedAction unpause;

        /// <summary>
        /// <inheritdoc/>
        /// Copy the method references too.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override object Clone()
        {
            var copy = (CustomAction)base.Clone();
            copy.start = (ContextualSerializedAction)start?.Clone();
            copy.update = (ContextualSerializedStatusFunction)update?.Clone();
            copy.stop = (ContextualSerializedAction)stop?.Clone();
            copy.pause = (ContextualSerializedAction)pause?.Clone();
            copy.unpause = (ContextualSerializedAction)pause?.Clone();
            return copy;
        }

        public override string ToString()
        {
            List<string> actionLines = new List<string>();
            string startLine = start.ToString();
            if (!string.IsNullOrEmpty(startLine)) actionLines.Add($"Start:{startLine}");
            string updateLine = update.ToString();
            if (!string.IsNullOrEmpty(updateLine)) actionLines.Add($"Update:{updateLine}");
            string stopLine = stop.ToString();
            if (!string.IsNullOrEmpty(stopLine)) actionLines.Add($"Stop:{stopLine}");
            string pauseLine = pause.ToString();
            if (!string.IsNullOrEmpty(pauseLine)) actionLines.Add($"Pause:{pauseLine}");
            string unpauseLine = unpause.ToString();
            if (!string.IsNullOrEmpty(unpauseLine)) actionLines.Add($"Unpause:{unpauseLine}");

            return "CustomAction(" + string.Join(", ", actionLines) + ")";
        }

        public void Build(BSBuildingInfo data)
        {
            onStarted = start.CreateDelegate(data.Runner);
            onUpdated = update.CreateDelegate(data.Runner);
            onStopped = stop.CreateDelegate(data.Runner);
            onPaused = pause.CreateDelegate(data.Runner);
            onUnpaused = unpause.CreateDelegate(data.Runner);
        }

        public bool Validate(BSValidationInfo validationInfo) => true;
    }
}
