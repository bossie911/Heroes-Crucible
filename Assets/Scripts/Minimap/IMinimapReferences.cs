using UnityEngine;

namespace GameStudio.HunterGatherer.Minimap
{
    ///<summary>Interface for minimap references</summary>
    internal interface IMinimapReferences
    {
        bool MinimapZeroInvertX { get; }
        bool MinimapZeroInvertY { get; }
        Camera PlayerCamera { get; }
        Transform WorldCenterTransform { get; }
        Camera DepthCamera { get; }

        Vector2 NormalizeWorldSpacePosition(Vector3 position);
        Rect RectTransformToScreenSpace(RectTransform transform);
    }
}