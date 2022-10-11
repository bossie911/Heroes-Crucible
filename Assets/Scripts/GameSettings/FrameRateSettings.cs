using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.GameSettings
{
    /// <summary> Class for setting target framerate </summary>
    public class FrameRateSettings : MonoBehaviour
    {
        [SerializeField]
        Dropdown FrameRateDropDown = null;

        [SerializeField, Header("Note, This modifies the default value only. The variable will be overriden by the saved PlayerPreferences variable.")]
        private int DefaultTargetFPS = 60;
        private int targetFPS = 0;

        private const string key = "TargetFPS";

        public int TargetFPS
        {
            get
            {
                return targetFPS;
            }
            set
            {
                targetFPS = value;
                Application.targetFrameRate = value;
            }
        }

        private void Start()
        {
            // Obtain the saved value
            int fps = DefaultTargetFPS;
            if (PlayerPrefs.HasKey(key))
            {
                fps = PlayerPrefs.GetInt(key);
            }
            TargetFPS = fps;

            // Update the UI
            if (FrameRateDropDown != null)
            {
                FrameRateDropDown.value = GetPreferedFPSOption(fps);
            }
        }

        /// <summary> Sets the prefered target framerate and saves it to the PlayerPreferences file. </summary>
        public void SetTargetFrameratePreference(int option)
        {
            int tFPS;
            switch (option)
            {
                case 1:
                    tFPS = 30;
                    break;
                case 2:
                    tFPS = 60;
                    break;
                case 3:
                    tFPS = 120;
                    break;
                default:
                    tFPS = -1;
                    break;
            }
            // Ignore if new target FPS is already the existing FPS
            if (tFPS == TargetFPS)
            {
                return;
            }
            Debug.Log($"Set PlayerPrefence {key} to {tFPS}");
            PlayerPrefs.SetInt(key, tFPS);
            TargetFPS = tFPS;
        }

        /// <summary>Gets the UI setting based on the amount of fps</summary>
        private int GetPreferedFPSOption(int fps)
        {
            switch (fps)
            {
                case 30:
                    return 1;
                case 60:
                    return 2;
                case 120:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}