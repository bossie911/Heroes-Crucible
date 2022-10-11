using UnityEngine;
using TMPro;
using GameStudio.HunterGatherer.GameTime;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    
    private void Start()
    {
        GameTimeManager.Instance.OnSecondIncrement.AddListener(UpdateClockTime);
    }

    private void OnDestroy()
    {
        GameTimeManager.Instance.OnSecondIncrement.RemoveListener(UpdateClockTime);
    }

    private void UpdateClockTime(int totalSeconds)
    {
        float minutes = Mathf.Floor(totalSeconds / 60);
        float seconds = totalSeconds % 60;
        
        text.text = minutes + ":" + seconds.ToString("00");
    }
}