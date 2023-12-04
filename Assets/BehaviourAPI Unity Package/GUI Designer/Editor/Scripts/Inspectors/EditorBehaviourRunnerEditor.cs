using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Runtime;

    [CustomEditor(typeof(EditorBehaviourRunner), editorForChildClasses: true)]
    public class EditorBehaviourRunnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorBehaviourRunner runner = (EditorBehaviourRunner)target;

            bool isPartOfAPrefab = PrefabUtility.IsPartOfAnyPrefab(runner);
            bool isOnScene = runner.gameObject.scene.name != null;
            bool isOnPreviewScene = isOnScene && EditorSceneManager.IsPreviewScene(runner.gameObject.scene);

            if (isOnScene)
            {
                string btnText = Application.isPlaying ? "OPEN DEBUGGER" : "OPEN EDITOR";
                if (GUILayout.Button(btnText))
                {
                    BehaviourSystemEditorWindow.Create(runner, runtime: Application.isPlaying);
                }
            }
            else
            {
                // Edit system from asset view throw error.
                EditorGUILayout.HelpBox("Enter the prefab to edit the behaviour system.", MessageType.Info);
            }


            if (isPartOfAPrefab && !isOnPreviewScene)
                EditorGUILayout.HelpBox("If you edit the behaviourSystem in a prefab instance, the original system will be override", MessageType.Info);
        }
    }
}
