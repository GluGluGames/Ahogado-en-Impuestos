using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{
    using Framework;

    /// <summary>
    /// Subclass of  <see cref="BehaviourRunner"/> used to edit a behaviour runner with the editor tools.
    /// </summary>
    public class EditorBehaviourRunner : DataBehaviourRunner
    {
        #region ------------------------------- Private fields --------------------------------

        [SerializeField] SystemData data = new SystemData();

        #endregion

        #region --------------------------------- Properties ----------------------------------

        public override SystemData Data => data;

        #endregion

        #region --------------------------------- properties ----------------------------------

        protected sealed override SystemData GetEditedSystemData() => data;

        #endregion
    }
}
