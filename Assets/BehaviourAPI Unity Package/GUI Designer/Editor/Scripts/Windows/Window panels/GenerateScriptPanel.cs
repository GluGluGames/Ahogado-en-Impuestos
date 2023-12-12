using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using CodeGenerator;
    public class GenerateScriptPanel : ToolPanel
    {
        private TextField m_ScriptText;
        private TextField m_classNameText;
        private TextField m_pathText;
        private TextField m_namespaceText;

        private Toggle m_IncludeNodeNamesToggle;
        private Toggle m_RegisterGraphsInDebuggerToggle;
        private Toggle m_UseVarKeywordToggle;

        BehaviourSystemEditorWindow m_window;

        private CodeTemplate m_Template;

        public GenerateScriptPanel(BehaviourSystemEditorWindow customEditorWindow) : base("/generatescriptpanel.uxml")
        {
            m_window = customEditorWindow;
            m_ScriptText = this.Q<TextField>("gsp-text");
            m_classNameText = this.Q<TextField>("gsp-name-field");
            m_classNameText.RegisterValueChangedCallback(_ => RefreshGeneratedCode());
            m_pathText = this.Q<TextField>("gsp-path-field");
            m_namespaceText = this.Q<TextField>("gsp-namespace-field");
            m_namespaceText.RegisterValueChangedCallback(_ => RefreshGeneratedCode());

            var createBtn = this.Q<Button>("gsp-create-btn");
            createBtn.clicked += GenerateScriptAsset;

            m_IncludeNodeNamesToggle = this.Q<Toggle>("csw-includenodename-toggle");
            m_IncludeNodeNamesToggle.RegisterValueChangedCallback(_ => RefreshGeneratedCode());
            m_UseVarKeywordToggle = this.Q<Toggle>("csw-usevarkeyword-toggle");
            m_UseVarKeywordToggle.RegisterValueChangedCallback(_ => RefreshGeneratedCode());
        }

        public override void Open(bool canClose = true)
        {
            base.Open(canClose);
            m_Template = new CodeTemplate();
            m_Template.Create(m_window.System.Data);
            RefreshGeneratedCode();
        }

        private void RefreshGeneratedCode()
        {
            CodeGenerationOptions options = new CodeGenerationOptions()
            {
                scriptNamespace = m_namespaceText.text,
                includeNames = m_IncludeNodeNamesToggle.value,
                useVarKeyword = m_UseVarKeywordToggle.value,
            };
            m_ScriptText.value = m_Template.GenerateCode(m_classNameText.value, options);
        }

        private void GenerateScriptAsset()
        {
            if (string.IsNullOrEmpty(m_ScriptText.value)) return;

            var path = m_pathText.value;
            if (path[path.Length - 1] != '/') path += "/";

            var file = m_classNameText.value + ".cs";

            var fullPath = $"{path}{file}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            UTF8Encoding encoding = new UTF8Encoding(true, false);
            StreamWriter writer = new StreamWriter(Path.GetFullPath(fullPath), false, encoding);
            writer.Write(m_ScriptText.value);
            writer.Close();

            AssetDatabase.ImportAsset(path);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath);
            AssetDatabase.Refresh();
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }
}
