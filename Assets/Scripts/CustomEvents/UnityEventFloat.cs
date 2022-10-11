using UnityEngine.Events;

namespace GameStudio.HunterGatherer.CustomEvents
{
    /// <summary> UnityEvent with float parameter. Required so UnityEditor can render it in the inspector. </summary>
    [System.Serializable]
    public class UnityEventFloat : UnityEvent<float> { }
}