using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Reflection;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    class BehaviourAPISettingsProvider : SettingsProvider
    {
        class Styles
        {
            public static GUIContent RootPath = new GUIContent("Package root PATH");
            public static GUIContent Assemblies = new GUIContent("Assemblies");

            public static GUIContent ScriptPath = new GUIContent("Default script PATH");
            public static GUIContent ScriptName = new GUIContent("Default script name");
        }

        SerializedObject m_SerializedObject;

        SerializedProperty m_assemblies;

        public BehaviourAPISettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            BehaviourAPISettings.instance.Save();
            m_SerializedObject = new SerializedObject(BehaviourAPISettings.instance);
            m_assemblies = m_SerializedObject.FindProperty("m_assemblies");
        }
        public override void OnGUI(string searchContext)
        {
            using (CreateSettingsWindowGUIScope())
            {
                m_SerializedObject.Update();
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

                var pathProp = m_SerializedObject.FindProperty("RootPath");
                if (pathProp != null)
                {
                    pathProp.stringValue = EditorGUILayout.TextField(Styles.RootPath, pathProp.stringValue);
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Script generation", EditorStyles.boldLabel);

                var scriptPathProp = m_SerializedObject.FindProperty("GenerateScriptDefaultPath");
                if (scriptPathProp != null)
                {
                    scriptPathProp.stringValue = EditorGUILayout.TextField(Styles.ScriptName, scriptPathProp.stringValue);
                }

                var scriptNameProp = m_SerializedObject.FindProperty("GenerateScriptDefaultName");
                if (scriptNameProp != null)
                {
                    scriptNameProp.stringValue = EditorGUILayout.TextField(Styles.ScriptName, scriptNameProp.stringValue);
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);

                EditorGUILayout.Space(5);
                DisplayColorProperty("LeafNodeColor");
                DisplayColorProperty("DecoratorColor");
                DisplayColorProperty("CompositeColor");

                EditorGUILayout.Space(5);
                DisplayColorProperty("StateColor");
                DisplayColorProperty("TransitionColor");

                EditorGUILayout.Space(5);
                DisplayColorProperty("LeafFactorColor");
                DisplayColorProperty("CurveFactorColor");
                DisplayColorProperty("FusionFactorColor");
                DisplayColorProperty("SelectableNodeColor");
                DisplayColorProperty("BucketColor");

                if (EditorGUI.EndChangeCheck())
                {
                    m_SerializedObject.ApplyModifiedProperties();
                    BehaviourAPISettings.instance.Save();
                }
            }
        }

        private void DisplayColorProperty(string propertyName)
        {
            var prop = m_SerializedObject.FindProperty(propertyName);
            if(prop != null)
            {
                prop.colorValue = EditorGUILayout.ColorField(new GUIContent(propertyName), prop.colorValue);
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateMySingletonProvider()
        {
            var provider = new BehaviourAPISettingsProvider("Project/BehaviourAPI settings", SettingsScope.Project, GetSearchKeywordsFromGUIContentProperties<Styles>());
            return provider;
        }

        private IDisposable CreateSettingsWindowGUIScope()
        {
            var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var type = unityEditorAssembly.GetType("UnityEditor.SettingsWindow+GUIScope");
            return Activator.CreateInstance(type) as IDisposable;
        }
    }
}