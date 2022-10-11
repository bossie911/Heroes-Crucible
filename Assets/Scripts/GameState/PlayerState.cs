using System;

namespace GameStudio.HunterGatherer.GameState
{
    /// <summary>The state of the player</summary>
    public enum PlayerState
    {
        Loading = 1 << 0,
        Won = 1 << 1,
        Draw = 1 << 2,
        Playing = 1 << 3,
        Lost = 1 << 4,
        Spectating = 1 << 5
    }
}