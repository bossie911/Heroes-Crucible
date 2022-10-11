using GameStudio.HunterGatherer.Divisions;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.CustomEvents
{
    /// <summary>Custom UnityEvent that passes a division goal</summary>
    [System.Serializable]
    public class UnityEventDivisionGoal : UnityEvent<DivisionGoal> { }
}
