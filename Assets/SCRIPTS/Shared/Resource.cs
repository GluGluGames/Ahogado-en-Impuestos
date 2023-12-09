using UnityEngine.Localization;
using UnityEngine;

namespace GGG.Shared
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Game/Resource")]
    public class Resource : ScriptableObject
    {
        [SerializeField] private string Key;
        [SerializeField] private LocalizedString Name;
        [SerializeField] private LocalizedString Description;
        [SerializeField] private Sprite Sprite;
        [SerializeField] private Sprite SelectedSprite;
        [SerializeField] private GameObject Model;
        [SerializeField] private Vector3 ModelScale;
        [SerializeField] private bool CanBeResearched;
        [SerializeField] private bool IsUnlocked;
        [Tooltip("Time in seconds")]
        [SerializeField] private int ResearchTime;

        public string GetKey() => Key;
        public string GetName() => Name.GetLocalizedString();
        public string GetDescription() => Description.GetLocalizedString();
        public GameObject GetModel() => Model;
        public Vector3 GetModelScale() => ModelScale;
        public Sprite GetSprite() => Sprite;
        public Sprite GetSelectedSprite() => SelectedSprite;
        public void DiscoverResource() => CanBeResearched = true;
        public bool Unlocked() => IsUnlocked;
        public void Unlock() => IsUnlocked = true;
        public bool CanResearch() => CanBeResearched;
        public int GetResearchTime() => ResearchTime;
    }
}
