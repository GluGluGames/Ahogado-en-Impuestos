using System;
using GGG.Classes.Buildings;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace GGG.Components.Buildings.Museum
{
    public class MuseumDescription : MonoBehaviour
    {
        [SerializeField] private TMP_Text Title;
        [SerializeField] private TMP_Text Description;
        [SerializeField] private LocalizedString DefaultDescription;

        private void OnEnable()
        {
            Title.SetText(DefaultDescription.GetLocalizedString());
            Description.SetText(DefaultDescription.GetLocalizedString());
        }

        public void SetText(Resource resource, Building building)
        {
            if ((resource && !resource.Unlocked()) || (building && !building.IsUnlocked()))
            {
                Title.SetText(DefaultDescription.GetLocalizedString());
                Description.SetText(DefaultDescription.GetLocalizedString());
                return;
            }
            
            Title.SetText(resource ? resource.GetName() : building.GetName());
            Description.SetText(resource ? resource.GetDescription() : building.GetDescription());
        }
    }
}
