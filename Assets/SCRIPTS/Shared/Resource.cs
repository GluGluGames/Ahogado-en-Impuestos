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
        public void DiscoverResource() => PlayerPrefs.SetInt($"Research{Key}", 1);

        public bool Unlocked()
        {
            if (PlayerPrefs.HasKey(Key)) 
                return PlayerPrefs.GetInt(Key) == 1;

            return IsUnlocked;
        }
        public void Unlock() => PlayerPrefs.SetInt(Key, 1);
        public bool CanResearch()
        {
            if (PlayerPrefs.HasKey($"Research{Key}")) return PlayerPrefs.GetInt($"Research{Key}") == 1;

            return CanBeResearched;
        }
        public int GetResearchTime() => ResearchTime;
    }
}
