using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.GameSettings
{
    public class SoundSettings : MonoBehaviour
    {
        private FMOD.Studio.Bus master;
        private FMOD.Studio.Bus sfx;
        private FMOD.Studio.Bus music;

        private float masterVolume = .7f;
        private float sfxVolume = .7f;
        private float musicVolume = .7f;

        [SerializeField] private PlayOneShot sfxTestAudio;

        public Slider masterSlider;
        public Slider sfxSlider;
        public Slider musicSlider;

        private const string masterKey = "masterVolume";
        private const string sfxKey = "sfxVolume";
        private const string musicKey = "musicVolume";

        public void LoadValues(SettingSliders sliders)
        {
            // Fill sliders if their not filled
            if(masterSlider == null)
            {
                masterSlider = sliders.masterSlider;
                sfxSlider = sliders.sfxSlider;
                musicSlider = sliders.musicSlider;
            }
            
            // Adding listeners
            masterSlider.onValueChanged.AddListener(MasterVolumeLevel);
            sfxSlider.onValueChanged.AddListener(SfxVolumeLevel);
            musicSlider.onValueChanged.AddListener(MusicVolumeLevel);
            
            // Getting busses to change the volume
            master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
            sfx = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
            music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");

            // Load player pref data if found
            masterVolume = PlayerPrefs.GetFloat(masterKey, .7f);
            sfxVolume = PlayerPrefs.GetFloat(sfxKey, .7f);
            musicVolume = PlayerPrefs.GetFloat(musicKey, .7f);

            // Visualising pref data to match the slider
            masterSlider.value = masterVolume;
            sfxSlider.value = sfxVolume;
            musicSlider.value = musicVolume;

            // Change the volume of the busses
            master.setVolume(masterVolume);
            sfx.setVolume(sfxVolume);
            music.setVolume(musicVolume);
        }

        public void MasterVolumeLevel(float newMasterLevel)
        {
            PlayerPrefs.SetFloat(masterKey, newMasterLevel);
            masterVolume = newMasterLevel;
            master.setVolume(masterVolume);
        }

        public void SfxVolumeLevel(float newSfxLevel)
        {
            PlayerPrefs.SetFloat(sfxKey, newSfxLevel);
            sfxVolume = newSfxLevel;
            sfx.setVolume(sfxVolume);
            sfxTestAudio.Play();
        }

        public void MusicVolumeLevel(float newMusicLevel)
        {
            PlayerPrefs.SetFloat(musicKey, newMusicLevel);
            musicVolume = newMusicLevel;
            music.setVolume(musicVolume);
        }
    }
}
