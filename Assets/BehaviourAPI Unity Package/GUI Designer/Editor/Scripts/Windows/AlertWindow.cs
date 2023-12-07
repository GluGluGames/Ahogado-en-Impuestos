using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    public class AlertWindow : EditorWindow
    {
        private static string path => "windows/alertwindow.uxml";

        private static readonly Vector2 k_Offset = new Vector2(50,50);

        public static string Question;

        public static Action OnPressYes, OnPressNo;
        public static void CreateAlertWindow(string question, Vector2 pos, Action onPressYes, Action onPressNo = null)
        {
            Question = question;
            OnPressYes = onPressYes;
            OnPressNo = onPressNo;

            AlertWindow wnd = GetWindow<AlertWindow>();
            wnd.titleContent = new GUIContent("AlertWindow");

            wnd.minSize = new Vector2(300, 150);
            wnd.maxSize = new Vector2(300, 150);

            wnd.position = new Rect(pos + k_Offset, wnd.minSize);

            wnd.ShowModalUtility();

        }

        public void CreateGUI()
        {
            var windownFromUXML = BehaviourAPISettings.instance.GetLayoutAsset(path);
            windownFromUXML.CloneTree(rootVisualElement);

            var label = rootVisualElement.Q<Label>("aw-question-label");
            var yesBtn = rootVisualElement.Q<Button>("aw-yes-btn");
            var noBtn = rootVisualElement.Q<Button>("aw-no-btn");

            label.text = Question;
            yesBtn.clicked += () => { OnPressYes?.Invoke(); Close(); };
            noBtn.clicked += () => { OnPressNo?.Invoke(); Close(); };
        }
    }
}