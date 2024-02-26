using System;
using GGG.Components.Achievements;
using GGG.Components.Core;
using GGG.Components.Scenes;
using GGG.Components.Serialization;
using GGG.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.UI.Lateral
{
    public class LateralExpeditionButton : MonoBehaviour
    {
        public Action OnExpedition;
        
        public void OnExpeditionButton()
        {
            if (GameManager.Instance.OnTutorial()) return;
            SerializationManager.Instance.Save();

            OnExpedition?.Invoke();
            
            SceneIndexes sceneIndex = Random.Range(1, 5) switch
            {
                1 => SceneIndexes.MINIGAME_LEVEL1,
                2 => SceneIndexes.MINIGAME_LEVEL2,
                3 => SceneIndexes.MINIGAME_LEVEL3,
                4 => SceneIndexes.MINIGAME_LEVEL4,
                _ => SceneIndexes.MINIGAME_LEVEL1
            };

            SceneManagement.Instance.LoadScene(sceneIndex, SceneIndexes.GAME_SCENE);
            
            int x = PlayerPrefs.HasKey("Achievement09") ? PlayerPrefs.GetInt("Achievement09") + 1 : 1;
            PlayerPrefs.SetInt("Achievement09", x);

            if (PlayerPrefs.GetInt("Achievement09") >= 5)
                StartCoroutine(AchievementsManager.Instance.UnlockAchievement("09"));
        }
    }
}
