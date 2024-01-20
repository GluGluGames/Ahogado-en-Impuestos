using System;
using GGG.Components.Core;
using GGG.Components.Scenes;
using GGG.Input;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour
{
    private InputManager _input;
    private SceneManagement _sceneManagement;
    
    private void Start()
    {
        _input = InputManager.Instance;
        _sceneManagement = SceneManagement.Instance;
    }

    private void Update()
    {
        if (!_input.Escape()) return;
        
        // _sceneManagement.OpenSettings();
    }
}
