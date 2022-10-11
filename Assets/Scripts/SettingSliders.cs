using System;
using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer.GameSettings;
using UnityEngine;
using UnityEngine.UI;

public class SettingSliders : MonoBehaviour
{
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    public void Start()
    {
        FindObjectOfType<SoundSettings>().LoadValues(this);
    }
}
