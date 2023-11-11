using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    [InitializeOnLoad]
    public static class EditorSettings
    {
        static EditorSettings()
        {
            EditorApplication.playModeStateChanged += RefreshBehaviourEditorWindow;
            EditorSceneManager.sceneOpened += ClearBehaviourWindow;
            BehaviourAPISettings.instance.ReloadAssemblies();
        }

        private static void ClearBehaviourWindow(Scene scene, OpenSceneMode mode)
        {
            if (BehaviourSystemEditorWindow.instance != null)
                BehaviourSystemEditorWindow.instance.OnChangeOpenScene();
        }

        static void RefreshBehaviourEditorWindow(PlayModeStateChange playModeStateChange)
        {
            if (BehaviourSystemEditorWindow.instance != null)
                BehaviourSystemEditorWindow.instance.OnChangePlayModeState(playModeStateChange);
        }


    }
}
