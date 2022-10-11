using GameStudio.HunterGatherer.FogOfWar;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.CustomEvents
{
    /// <summary>Custom UnityEvent that passes a FogOfWarMask</summary>
    [System.Serializable]
    public class UnityEventFogOfWarMask : UnityEvent<FogOfWarMask> { }
}
