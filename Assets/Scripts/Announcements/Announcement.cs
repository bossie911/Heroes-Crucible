using UnityEngine;

namespace GameStudio.HunterGatherer.UI
{
    /// <summary> This scriptable object is used to add a string message on the GUI </summary>
    [CreateAssetMenu(fileName = "Announcement", menuName = "ScriptableObjects/Announcement")]
    public class Announcement : ScriptableObject
    {
        [SerializeField]
        private string text = null;

        public string Text => text;
    }
}

