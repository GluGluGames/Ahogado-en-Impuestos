using GGG.Components.UI;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float InitialTime;

    private TextMeshProUGUI _timerText;
    private float _timerDelta;

    private bool paused = true;

    private void OnEnable()
    {
        _timerText = GetComponentInParent<TextMeshProUGUI>();
        ResetTimer();
        paused = false;
    }

    private void Update()
    {
        if (paused) return;

        _timerDelta -= Time.deltaTime;

        if (_timerDelta <= 0) Win(true);

        int minutes = Mathf.FloorToInt(_timerDelta / 60);
        int seconds = Mathf.FloorToInt(_timerDelta % 60);
        _timerText.text = $"{minutes:00}:{seconds:00}";        
    }

    private void ResetTimer()
    {
        _timerText.text = $"{InitialTime:0}";
        _timerText.gameObject.SetActive(true);
        InitialTime = InitialTime * 60;
        _timerDelta = InitialTime;
    }

    public void Win(bool win)
    {
        Pause();   
        _timerText.gameObject.SetActive(false);
        FindObjectOfType<EndExpeditionUI>().OnEndGame(win);
    }

    public void Pause()
    {
        paused = true;
    }

}
