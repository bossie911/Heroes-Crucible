using System;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Enum describing the current goal of the division</summary>
    [Flags]
    public enum DivisionGoal
    {
        Idle = 1,
        Move = 2,
        Attack = 4,
        Defend = 8,
        PlaceBase = 16
    }
}