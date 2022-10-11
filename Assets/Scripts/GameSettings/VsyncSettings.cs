using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.GameSettings
{
    /// <summary> Class for setting VSync </summary>
    public class VsyncSettings : MonoBehaviour
    {
        [SerializeField]
        Toggle VsyncToggle = null;

        /// <summary> 0 = off. 1 = Every v blank. 2 = Every second v blank. </summary>
        [SerializeField, Header("Overriden by PlayerPrefs")]
        int defaultVsyncOption = 1;

        private const string key = "Vsync";
        int _vsync;

        int Vsync
        {
            get
            {
                return _vsync;
            }
            set
            {
                if (value > 2 || value < 0)
                {
                    return;
                }
                _vsync = value;
                QualitySettings.vSyncCount = value;
            }
        }

        private void Start()
        {
            // Obtain the saved value
            int vsyncOption = defaultVsyncOption;
            if (PlayerPrefs.HasKey(key))
            {
                vsyncOption = PlayerPrefs.GetInt(key);
            }
            Vsync = vsyncOption;

            // Update the UI
            if (VsyncToggle != null)
            {
                VsyncToggle.isOn = GetPreferedVsyncOption(vsyncOption);
            }
        }

        /// <summary> Sets the vsync and saves it in the PlayerPrefs </summary>
        /// <param name="state"></param>
        public void SetVsyncPreference(bool state)
        {
            int vsyncOption = (state) ? 1 : 0;
            // Ignore the changes if it's already the same.
            if (vsyncOption == Vsync)
            {
                return;
            }

            // Ignore if new target FPS is already the existing FPS
            PlayerPrefs.SetInt(key, vsyncOption);
            Debug.Log($"Set PlayerPrefence {key} to {vsyncOption}");
            Vsync = vsyncOption;
        }

        /// <summary> Gets the UI state based on the settings. </summary>
        private bool GetPreferedVsyncOption(int state)
        {
            return (state != 0);
        }
    }
}