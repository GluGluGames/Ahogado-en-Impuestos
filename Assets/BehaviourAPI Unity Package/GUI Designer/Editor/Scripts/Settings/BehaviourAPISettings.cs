using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    [FilePath("ProjectSettings/BehaviourAPISettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BehaviourAPISettings : ScriptableSingleton<BehaviourAPISettings>
    {
        #region ------------------------- Default values ------------------------

        private static readonly Color k_LeafNodeColor = new Color(1f, 0.45f, 0.15f, 1f);
        private static readonly Color k_DecoratorColor = new Color(0.35f, 0.95f, 0.65f);
        private static readonly Color k_CompositeColor = new Color(0.25f, 0.7f, 0.9f, 1f);
        private static readonly Color k_StateColor = new Color(1f, 0.45f, 0.15f, 1f);
        private static readonly Color k_TransitionColor = new Color(0.65f, 0.7f, 0.75f, 1f);
        private static readonly Color k_LeafFactorColor = new Color(0.45f, 0.65f, 1f, 1f);
        private static readonly Color k_CurveFactorColor = new Color(0.4f, 0.4f, 0.75f, 1f);
        private static readonly Color k_FusionFactorColor = new Color(0.7f, 0.3f, 0.7f, 1f);
        private static readonly Color k_SelectableNodeColor = new Color(1f, 0.45f, 0.15f, 1f);
        private static readonly Color k_BucketColor = new Color(0.7f, 0.3f, 0.25f, 1f);

        private static readonly string[] k_DefaultAssemblies = new[]
        {
            "Assembly-CSharp",
            "BehaviourAPI.Core",
            "BehaviourAPI.StateMachines",
            "BehaviourAPI.BehaviourTrees",
            "BehaviourAPI.UtilitySystems",
            "BehaviourAPI.UnityToolkit",
            "BehaviourAPI.UnityToolkit.GUIDesigner.Runtime",
            "BehaviourAPI.UnityToolkit.GUIDesigner.Framework",
            "BehaviourAPI.UnityToolkit.GUIDesigner.Editor"
        };

        private static readonly string k_RootPath = "Assets/BehaviourAPI Unity Package/GUI Designer";
        #endregion

        #region ----------------------- Editor settings -----------------------

        [SerializeField] private string RootPath = k_RootPath;

        public string GenerateScriptDefaultPath = "Assets/Scripts/";
        public string GenerateScriptDefaultName = "NewBehaviourRunner";

        [Header("Colors")]
        public Color LeafNodeColor = k_LeafNodeColor;
        public Color DecoratorColor = k_DecoratorColor;
        public Color CompositeColor = k_CompositeColor;

        public Color StateColor = k_StateColor;
        public Color TransitionColor = k_TransitionColor;

        public Color LeafFactorColor = k_LeafFactorColor;
        public Color CurveFactorColor = k_CurveFactorColor;
        public Color FusionFactorColor = k_FusionFactorColor;
        public Color SelectableNodeColor = k_SelectableNodeColor;
        public Color BucketColor = k_BucketColor;

        #endregion

        /// <summary>
        /// Root path of editor layout elements
        /// </summary>
        public string EditorLayoutsPath => $"{RootPath}/Editor/Resources/uxml/";

        /// <summary>
        /// Root path of editor style sheets
        /// </summary>
        public string EditorStylesPath => $"{RootPath}/Editor/Resources/uss/";

        /// <summary>
        /// Root path of the script templates
        /// </summary>
        public string ScriptTemplatePath => $"{RootPath}/Editor/Resources/Templates/";

        /// <summary>
        /// 
        /// </summary>
        public string IconPath => $"{RootPath}/Editor/Resources/Icons/";

        /// <summary>
        /// 
        /// </summary>
        public APITypeMetadata Metadata;

        public void Save() => Save(true);

        /// <summary>
        /// Create the type hierarchies used in the editor when Unity reloads.
        /// </summary>
        public void ReloadAssemblies()
        {
            Metadata = new APITypeMetadata();

            if (!System.IO.Directory.Exists(RootPath))
            {
                Debug.LogWarning("BehaviourAPISettings: Root path doesn't exist. Change the path in ProyectSetting > BehaviourAPI");
            }
        }

        /// <summary>
        /// return a uxml file from the layouts folder
        /// </summary>
        /// <param name="subpath">The relative path to the <see cref="EditorLayoutsPath"/> folder.</param>
        /// <returns>The asset in the specified path.</returns>
        public VisualTreeAsset GetLayoutAsset(string subpath)
        {
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorLayoutsPath + subpath);
        }

        /// <summary>
        /// return a uss file from the styles folder
        /// </summary>
        /// <param name="subpath">The relative path to the <see cref="EditorStylesPath"/> folder.</param>
        /// <returns>The asset in the specified path.</returns>
        public StyleSheet GetStyleSheet(string subpath)
        {
            return AssetDatabase.LoadAssetAtPath<StyleSheet>(EditorStylesPath + subpath);
        }
    }
}