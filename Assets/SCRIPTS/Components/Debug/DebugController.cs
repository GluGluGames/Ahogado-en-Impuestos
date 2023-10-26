using System;
using System.Collections.Generic;
using GGG.Classes.Debug;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Input;
using UnityEngine;

namespace GGG.Components.Debug
{
    public class DebugController : MonoBehaviour
    {
        private static DebugCommand HELP;
        private static DebugCommand<string, int> GET_RESOURCE;
        private static List<object> _commandList;
        
        private InputManager _inputManager;
        private GameManager _gameManager;
        private PlayerManager _playerManager;

        private Vector2 _scroll;
        
        private bool _showConsole;
        private bool _showHelp;
        
        private string _input;
        private void Start()
        {
            _inputManager = InputManager.Instance;
            _gameManager = GameManager.Instance;
            _playerManager = PlayerManager.Instance;
            
            InitializeCommands();
        }

        private void Update()
        {
            if (_inputManager.DebuConsole()) OnDebugConsole();
            if(_inputManager.EnterKey()) OnEnterKey();
        }

        private void OnEnterKey()
        {
            if (!_showConsole) return;
            
            HandleInput();
            _input = "";
        }

        private void OnDebugConsole()
        {
            _showConsole = !_showConsole;
            if(_showConsole) _gameManager.OnUIOpen();
            else _gameManager.OnUIClose();
        }

        private void InitializeCommands()
        {
            GET_RESOURCE = new DebugCommand<string, int>("get_resource",
                "Gets a resource", 
                "get_resource <name> <number>",
                (resourceKey, resourceNumber) =>
                {
                    _playerManager.AddResource(resourceKey, resourceNumber);
                });

            HELP = new DebugCommand("help", "Show the list of commands", "help", () =>
            {
                _showHelp = true;
            });

            _commandList = new List<object>
            {
                HELP,
                GET_RESOURCE,
            };
        }

        private void OnGUI()
        {
            if (!_showConsole) return;

            float y = 0;

            if (_showHelp)
            {
                GUI.Box(new Rect(0, y, Screen.width, 100), "");

                Rect viewport = new Rect(0, 0, Screen.width - 30f, 20 * _commandList.Count);
                _scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), _scroll, viewport);

                for (int i = 0; i < _commandList.Count; i++)
                {
                    DebugCommandBase command = _commandList[i] as DebugCommandBase;

                    string label = $"{command.GetFormat} - {command.GetDescription}";
                    Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                    GUI.Label(labelRect, label);
                }
                
                GUI.EndScrollView();
                y += 100;
            }
            
            GUI.Box(new Rect(0, y, Screen.width, 40), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            GUI.SetNextControlName("console");
            _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 40f), _input);
            GUI.FocusControl("console");
        }

        private void HandleInput()
        {
            string[] properties = _input.Split(' ');
            
            for (int i = 0; i < _commandList.Count; i++)
            {
                DebugCommandBase commandBase = _commandList[i] as DebugCommandBase;

                if (_input.Contains(commandBase.GetId))
                {
                    if (_commandList[i] is DebugCommand)
                    {
                        (_commandList[i] as DebugCommand).InvokeCommand();
                    }
                    else if (_commandList[i] is DebugCommand<string, int>)
                    {
                        (_commandList[i] as DebugCommand<string, int>).InvokeCommand(properties[1], int.Parse(properties[2]));
                    }
                }
            }
        }
    }
}
