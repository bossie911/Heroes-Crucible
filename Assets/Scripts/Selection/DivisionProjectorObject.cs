using GameStudio.HunterGatherer.Divisions;

using UnityEngine;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary> Container object for Divisions and their projectors. </summary>
    public struct DivisionProjectorObject
    {
        public Projector DivisionProjector { get; set; }
        public Projector DirectionProjector { get; set; }
        public Division Division { get; set; }
    }
}