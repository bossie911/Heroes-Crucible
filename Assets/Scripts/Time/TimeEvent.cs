using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.GameTime
{
    [Serializable]
    ///<summary> This class is used to make an object of an event and the time it will be invoked.</summary>
    public class TimeEvent
    {
        [Tooltip("The event that will be triggered when the specified time is reached.")]
        public UnityEvent onTimeMatched = new UnityEvent();
        
        [Tooltip("The time when the event should trigger.")]
        public int second;
    }
}
