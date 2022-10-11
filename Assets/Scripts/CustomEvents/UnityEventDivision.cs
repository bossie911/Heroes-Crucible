using System;
using System.Collections;
using System.Collections.Generic;

using GameStudio.HunterGatherer.Divisions;

using UnityEngine;
using UnityEngine.Events;

namespace GameStudio.HunterGatherer.CustomEvents
{
    /// <summary>Custom UnityEvent that passes a Division object</summary>
    [System.Serializable]
    public class UnityEventDivision : UnityEvent<Division>{}
}
