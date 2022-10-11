using UnityEngine;

namespace GameStudio.HunterGatherer.UI
{
    /// <summary>Button used to exit the game</summary>
    public class ButtonExitGame : MonoBehaviour
    {
        /// <summary>Invoke exit game</summary>
        public void ExitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
    }
}