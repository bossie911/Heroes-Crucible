using GameStudio.HunterGatherer.Divisions;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.CustomEvents
{
    /// <summary>Custom UnityEvent that passes a UnitState</summary>
    [System.Serializable]
    public class UnityEventUnitState : UnityEvent<UnitState, bool> { }
}
