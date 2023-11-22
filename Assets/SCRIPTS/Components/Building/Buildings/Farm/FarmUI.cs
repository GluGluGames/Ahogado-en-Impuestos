using System.Collections.Generic;
using UnityEngine;
using GGG.Shared;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Farm
{
    public class FarmUI : MonoBehaviour
    {
        private Resource[] _resources;
        private List<Button[]> _buttons;
        private GameObject _viewport;

        [SerializeField] private GameObject[] Panels;
        [SerializeField] private _type Type;

        private enum _type
        {
            Plants,
            Fish
        };

        [SerializeField] private Image SelectedImage;

        private int _active = 0;



        private void Awake()
        {
            _resources = Resources.LoadAll<Resource>(Type == 0 ? "SeaResources" : "FishResources");
            _buttons = new List<Button[]>();

            _viewport = transform.GetChild(0).gameObject;
            //_viewport.SetActive(false);

            //_viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        private void Start()
        {

            Initialize();

        }


        private void Initialize()
        {
            foreach (var panel in Panels)
            {
                _buttons.Add(panel.GetComponentsInChildren<Button>());
            }

            //6
            //12 - 6
            //button - resources 
            // 3
            int divide = _resources.Length / 2;
            int button = 0;
            foreach (var buttonArray in _buttons)
            {
                for (int i = 0; i < buttonArray.Length; i++)
                {
                    if (i < divide)
                    {
                        SpriteState aux = new()
                        {
                            selectedSprite = _resources[i].GetSelectedSprite(),
                            highlightedSprite = _resources[i].GetSelectedSprite(),
                            disabledSprite = _resources[i].GetSprite()
                        };

                        buttonArray[i].spriteState = aux;
                        buttonArray[i].image.sprite = buttonArray[i].spriteState.disabledSprite;

                        int index = i;
                        buttonArray[i].onClick.AddListener(() => SelectResource(button, i));
                    }
                    else buttonArray[i].gameObject.SetActive(false);
                }

                button++;
            }


            SelectResource(0, _active);
        }

        private void SelectResource(int button, int index)
        {
            _buttons[button][index].image.sprite = _buttons[button][index].spriteState.disabledSprite;
            _active = index;
            _buttons[button][index].image.sprite = _buttons[button][index].spriteState.selectedSprite;

            SelectedImage.sprite = _buttons[button][index].spriteState.disabledSprite;
        }
    }
}
