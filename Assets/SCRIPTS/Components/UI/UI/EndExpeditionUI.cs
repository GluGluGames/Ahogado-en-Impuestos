using UnityEngine.Localization;
using TMPro;
using UnityEngine;

namespace GGG.Components.UI
{
    public class EndExpeditionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI OutcomeText;
        [SerializeField] private TextMeshProUGUI SubtitleText;

        [SerializeField] private LocalizedString[] OutcomeString;
        [SerializeField] private LocalizedString[] SubtitleString;

        [SerializeField] private GameObject Viewport;

        public void OnEndGame(bool isWin)
        {
            Viewport.gameObject.SetActive(true);
            OutcomeText.text = OutcomeString[isWin ? 1 : 0].GetLocalizedString();
            SubtitleText.text = SubtitleString[isWin ? 1 : 0].GetLocalizedString();
        }
    }
}