using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Toggles between input 1 and 2 being selected</summary>
    public class TabToToggle : MonoBehaviour
    {
        [SerializeField]
        InputField inputfield1 = null;

        [SerializeField]
        InputField inputfield2 = null;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!inputfield1.isFocused)
                {
                    inputfield1.Select();
                }
                else
                {
                    inputfield2.Select();
                }
            }
        }
    }
}