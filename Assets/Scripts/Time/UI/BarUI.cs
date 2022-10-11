using UnityEngine;
using UnityEngine.UI;
using GameStudio.HunterGatherer.GameTime;

public class BarUI : MonoBehaviour
{
    [SerializeField] private Color safeColor;
    [SerializeField] private Color dangerColor;
    [SerializeField] private Color warningColor;
    
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject bar;

    /// <summary> Value that dictates how many units the bar should decrease in size every second. </summary>
    private float decreaseValue; 
    private bool allowBarUpdating = true;
    
    private void Start()
    {
        CalculateDecreaseValue();
        SetSafe();
        GameTimeManager.Instance.OnSecondIncrement.AddListener(UpdateBarSize);
        GameTimeManager.Instance.OnRoundIncrement.AddListener(EnableBarUpdating);
        GameTimeManager.Instance.OnRoundIncrement.AddListener(CalculateDecreaseValue);
        GameTimeManager.Instance.OnRoundIncrement.AddListener(SetSafe);
    }

    private void OnDestroy()
    {
        GameTimeManager.Instance.OnSecondIncrement.RemoveListener(UpdateBarSize);
        GameTimeManager.Instance.OnRoundIncrement.RemoveListener(EnableBarUpdating);
        GameTimeManager.Instance.OnRoundIncrement.RemoveListener(CalculateDecreaseValue);
        GameTimeManager.Instance.OnRoundIncrement.RemoveListener(SetSafe);
    }

    /// <summary> Update the bar scale according to time. </summary>
    private void UpdateBarSize(int second)
    {
        if (allowBarUpdating)
        {
            Vector3 scale = bar.transform.localScale;
            scale.x -= decreaseValue;
            bar.transform.localScale = scale;
        }
        
        if (second <= 0) //Disable bar updating to show large red bar
        {
            ResetBarSize();
            allowBarUpdating = false;
        }
    }
    
    private void EnableBarUpdating()
    {
        allowBarUpdating = true;
    }

    public void ResetBarSize()
    {
        bar.transform.localScale = new Vector3(1, 1, 1);
    }
    
    private void CalculateDecreaseValue()
    {
        decreaseValue = 1f / GameTimeManager.Instance.RoundTimeLeft;
    }

    /// <summary> Set the bar color to the safe color, should only be called when a round starts </summary>
    private void SetSafe()
    {
        barImage.color = safeColor;
    }

    public void SetDanger()
    {
        barImage.color = dangerColor;
    }
    
    public void SetWarning()
    {
        barImage.color = warningColor;
    }
}