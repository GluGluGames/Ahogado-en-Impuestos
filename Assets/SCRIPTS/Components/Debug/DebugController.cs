using GGG.Classes.Debug;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Components.Buildings.Museum;
using GGG.Components.Buildings.Shop;
using GGG.Components.Buildings.Laboratory;
using GGG.Components.Buildings.Generator;
using GGG.Components.Taxes;
using GGG.Input;
using GGG.Shared;

using UnityEngine;
using System.Collections.Generic;

namespace GGG.Components.Debug
{
    public class DebugController : MonoBehaviour
    {
        private static DebugCommand HELP;
        private static DebugCommand<string, int> GET_RESOURCE;
        private static DebugCommand SHOW_RESOURCES;
        private static DebugCommand OPEN_SHOP;
        private static DebugCommand OPEN_MUSEUM;
        private static DebugCommand TRIGGER_TAXES;
        private static List<object> _commandList;
        
        private InputManager _inputManager;
        private GameManager _gameManager;
        private PlayerManager _playerManager;

        private Vector2 _scroll;
        
        private bool _showConsole;
        private bool _showHelp;
        private bool _showResources;
        
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
            HELP = new DebugCommand("help", "Show the list of commands", "help", () =>
            {
                if (_showResources) _showResources = false;
                _showHelp = true;
            });
            
            GET_RESOURCE = new DebugCommand<string, int>("add_resource", "Adds (or subtract) a resource", "add_resource <name> <number>",
                (resourceKey, resourceNumber) => {
                    if (!_playerManager.GetResource(resourceKey)) return;
                    
                    OnDebugConsole();
                    _playerManager.AddResource(resourceKey, resourceNumber);
                });

            SHOW_RESOURCES = new DebugCommand("show_resources", "Shows the list of resources", "show_resources", () =>
            {
                if (_showHelp) _showHelp = false;
                _showResources = true;
            });

            OPEN_SHOP = new DebugCommand("open_shop", "Opens the shop", "open_shop", () =>
            {
                OnDebugConsole();
                FindObjectOfType<ShopUI>().OpenShop();
            });

            OPEN_MUSEUM = new DebugCommand("open_museum", "Opens the museum", "open_museum", () =>
            {
                OnDebugConsole();
                FindObjectOfType<MuseumUI>().Open();
            });

            TRIGGER_TAXES = new DebugCommand("trigger_taxes", "Poseidon will tax you", "trigger_taxes", () =>
            {
                OnDebugConsole();
                FindObjectOfType<TaxManager>().TriggerTaxes();
            });
                
            _commandList = new List<object>
            {
                HELP,
                GET_RESOURCE,
                SHOW_RESOURCES,
                OPEN_SHOP,
                OPEN_MUSEUM,
                TRIGGER_TAXES,
            };
        }

        private void OnGUI()
        {
            if (!_showConsole)
            {
                _showHelp = _showResources = false;
                return;
            }

            float y = 0;
            GUIStyle inputStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 24
            };

            if (_showHelp)
            {
                ShowHelp(y);
                y += 100;
            }

            if (_showResources)
            {
                ShowResources(y);
                y += 100;
            }
            
            GUI.Box(new Rect(0, y, Screen.width, 40), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            GUI.SetNextControlName("console");
            _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 40f), _input, inputStyle);
            GUI.FocusControl("console");
        }

        private void ShowResources(float y)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30f, 20 * _playerManager.GetResources().Count);
            _scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), _scroll, viewport);

            for (int i = 0; i < _playerManager.GetResources().Count; i++)
            {
                Resource resource = _playerManager.GetResources()[i];

                string label = $"{resource.GetKey()}";
                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                GUI.Label(labelRect, label, new GUIStyle{ fontSize = 20, normal = {textColor = Color.white}});
            }
                
            GUI.EndScrollView();
        }

        private void ShowHelp(float y)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30f, 20 * _commandList.Count);
            _scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), _scroll, viewport);

            for (int i = 0; i < _commandList.Count; i++)
            {
                DebugCommandBase command = _commandList[i] as DebugCommandBase;

                string label = $"{command.GetFormat} - {command.GetDescription}";
                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                GUI.Label(labelRect, label, new GUIStyle{ fontSize = 20, normal = {textColor = Color.white}});
            }
                
            GUI.EndScrollView();
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
