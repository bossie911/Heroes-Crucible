using GameStudio.HunterGatherer.Divisions;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.CustomEvents
{
    /// <summary>Custom UnityEvent that passes a list of divisions</summary>
    [System.Serializable]
    public class UnityEventDivisionList : UnityEvent<List<Division>> { }
}
