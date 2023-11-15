namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core.Perceptions;
    using System.Collections.Generic;

    /// <summary>
    /// Adaptation class for use custom <see cref="ConditionPerception"/> in editor tools.
    /// <para>! -- Don't use this class directly in code.</para>
    /// </summary>
    public class CustomPerception : ConditionPerception, IBuildable
    {
        /// <summary>
        /// Method reference for init event.
        /// </summary>
        public ContextualSerializedAction init;

        /// <summary>
        /// Method reference for check event.
        /// </summary>
        public ContextualSerializedBoolFunction check;

        /// <summary>
        /// Method reference for reset event.
        /// </summary>
        public ContextualSerializedAction reset;

        /// <summary>
        /// Method reference for reset event.
        /// </summary>
        public ContextualSerializedAction pause;

        /// <summary>
        /// Method reference for reset event.
        /// </summary>
        public ContextualSerializedAction unpause;

        /// <summary>
        /// <inheritdoc/>
        /// Copy the method references too.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override object Clone()
        {
            var copy = (CustomPerception)base.Clone();
            copy.init = (ContextualSerializedAction)init?.Clone();
            copy.check = (ContextualSerializedBoolFunction)check?.Clone();
            copy.reset = (ContextualSerializedAction)reset?.Clone();
            copy.pause = (ContextualSerializedAction)pause?.Clone();
            copy.unpause = (ContextualSerializedAction)unpause?.Clone();
            return copy;
        }

        public void Build(BSBuildingInfo data)
        {
            onInit = init.CreateDelegate(data.Runner);
            onCheck = check.CreateDelegate(data.Runner);
            onReset = reset.CreateDelegate(data.Runner);
            onPause = pause.CreateDelegate(data.Runner);
            onUnpause = unpause.CreateDelegate(data.Runner);
        }

        public override string ToString()
        {
            List<string> actionLines = new List<string>();
            string startLine = init.ToString();
            if (!string.IsNullOrEmpty(startLine)) actionLines.Add($"Init:{startLine}");
            string updateLine = check.ToString();
            if (!string.IsNullOrEmpty(updateLine)) actionLines.Add($"Check:{updateLine}");
            string stopLine = reset.ToString();
            if (!string.IsNullOrEmpty(stopLine)) actionLines.Add($"Reset:{stopLine}");
            string pauseLine = pause.ToString();
            if (!string.IsNullOrEmpty(pauseLine)) actionLines.Add($"Pause:{pauseLine}");
            string unpauseLine = unpause.ToString();
            if (!string.IsNullOrEmpty(unpauseLine)) actionLines.Add($"Unpause:{unpauseLine}");

            return "CustomAction(" + string.Join(", ", actionLines) + ")";
        }

        public bool Validate(BSValidationInfo validationInfo) => true;


    }
}
