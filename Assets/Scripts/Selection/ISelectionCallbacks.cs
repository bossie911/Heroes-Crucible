using System.Collections.Generic;

namespace GameStudio.HunterGatherer.Selection
{
    /// <summary>Can be implemented to listen to selection callbacks</summary>
    public interface ISelectionCallbacks
    {
        /// <summary>Called when selectable objects are selected</summary>
        void OnTargetsSelected(List<SelectableObject> selectableObjects);

        /// <summary>Called when selectable objects are deselected</summary>
        void OnTargetsDeselected(List<SelectableObject> selectableObjects);

        /// <summary>Called when a selectable object is starting to be hovered over</summary>
        void OnTargetHoverStart(SelectableObject selectableObject);

        /// <summary>Called when a selectable object is stopping to be hovered over</summary>
        void OnTargetHoverEnd(SelectableObject selectableObject);
    }
}