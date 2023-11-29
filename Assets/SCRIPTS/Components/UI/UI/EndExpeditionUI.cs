using GGG.Components.Core;
using UnityEngine.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GGG.Shared;

namespace GGG.Components.UI
{
    public class EndExpeditionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI OutcomeText;
        [SerializeField] private TextMeshProUGUI SubtitleText;

        [SerializeField] private LocalizedString[] OutcomeString;
        [SerializeField] private LocalizedString[] SubtitleString;

        [SerializeField] private GameObject Viewport;
        [SerializeField] private Button button;



        private void Start()
        {
            button.onClick.AddListener(() =>
            {
                HUDManager.Instance.ChangeScene(SceneIndexes.GAME_SCENE, SceneIndexes.MINIGAME);
                GameManager.Instance.OnUIClose();
            });
        }

        public void OnEndGame(bool isWin)
        {
            Viewport.gameObject.SetActive(true);
            OutcomeText.text = OutcomeString[isWin ? 0 : 1].GetLocalizedString();
            SubtitleText.text = SubtitleString[isWin ? 0 : 1].GetLocalizedString();
            GameManager.Instance.OnUIOpen();
        }


    }
}