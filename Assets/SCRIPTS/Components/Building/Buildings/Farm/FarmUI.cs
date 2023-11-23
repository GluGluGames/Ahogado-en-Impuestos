using System.Collections.Generic;
using UnityEngine;
using GGG.Shared;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Farm
{
    public class FarmUI : MonoBehaviour
    {
        private Resource[] _resources;
        private List<Button> _buttons;
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
            List<Button[]> auxButtons = new List<Button[]>();
            foreach (var panel in Panels)
            {
                auxButtons.Add(panel.GetComponentsInChildren<Button>());
            }

            _buttons = new List<Button>(auxButtons.Count * auxButtons[0].Length);

            for (int i = 0; i < auxButtons[0].Length; i++)
            {
                for (int j = 0; j < auxButtons.Count; j++)
                {
                    if (i < auxButtons[i].Length) _buttons[i * auxButtons.Count + j] = auxButtons[j][i];
                }
            }
        
            

            for (int i = 0; i < _buttons.Count; i++)
            {
                    SpriteState aux = new()
                    {
                        selectedSprite = _resources[i].GetSelectedSprite(),
                        highlightedSprite = _resources[i].GetSelectedSprite(),
                        disabledSprite = _resources[i].GetSprite()
                    };

                    _buttons[i].spriteState = aux;
                    _buttons[i].image.sprite = _buttons[i].spriteState.disabledSprite;

                    int index = i;
                    _buttons[i].onClick.AddListener(() => SelectResource(i));
                

            }
            
            //SelectResource(_active);
        }
    

        private void SelectResource(int index)
        {
            _buttons[index].image.sprite = _buttons[index].spriteState.disabledSprite;
            _active = index;
            _buttons[index].image.sprite = _buttons[index].spriteState.selectedSprite;

            SelectedImage.sprite = _buttons[index].spriteState.disabledSprite;
        }
    }
}
