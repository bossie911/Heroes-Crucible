using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace GameStudio.HunterGatherer.Audio
{
    public class PlayAudio : MonoBehaviour
    {
        [SerializeField]
        private EventReference eventReference;

        [SerializeField]
        private bool allowFadeOut = true;

        [SerializeField]
        private bool shouldPlayCertainTime = false;
        [SerializeField]
        private float playDuration = 3;
        [SerializeField]
        private int parameterAmount = 0;

        [SerializeField]
        private List<float> audioParametersValue = new List<float>();
        private List<PARAMETER_ID> audioParametersId = new List<PARAMETER_ID>();

        private EventInstance audioInstance;

        // Start is called before the first frame update
        void Start()
        {
            audioInstance = RuntimeManager.CreateInstance(eventReference);

            for (int i = 0; i < parameterAmount; i++)
            {
                EventDescription eventDescription;
                audioInstance.getDescription(out eventDescription);
                PARAMETER_DESCRIPTION parameterDescription;
                eventDescription.getParameterDescriptionByIndex(i, out parameterDescription);
                audioParametersId.Add(parameterDescription.id);
            }
        }

        public void Play()
        {
            if (shouldPlayCertainTime)
            {
                StartCoroutine(PlayCertainTimeEvent());
            }
            else
            {
                audioInstance.start();
                audioInstance.setParametersByIDs(audioParametersId.ToArray(), audioParametersValue.ToArray(), parameterAmount);
            }
        }

        private IEnumerator PlayCertainTimeEvent()
        {
            audioInstance.setParametersByIDs(audioParametersId.ToArray(), audioParametersValue.ToArray(), parameterAmount);
            audioInstance.start();
            yield return new WaitForSeconds(playDuration);
            Stop();
        }

        public void Stop()
        {
            if (allowFadeOut)
            {
                audioInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }
            else
            {
                audioInstance.stop(STOP_MODE.IMMEDIATE);
            }
        }
    }
}
