using GameStudio.HunterGatherer.CustomEvents;
using System.Collections;
using GameStudio.HunterGatherer.GameTime;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Environment.Flood
{
    /// <summary>Handles the flood over the course of the game</summary>
    public class Flood : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float durationOfFlood = 3f;
        [SerializeField]
        private float screenShakeDuration = 3f;
        [SerializeField]
        private float screenShakeIntensity = 3f;

        [SerializeField]
        private List<int> heightsOfSections = new List<int>();

        [SerializeField]
        private BoxCollider boxCollider;
        [SerializeField]
        private List<float> damagePerHeight = new List<float>();
        public float Damage => damagePerHeight[CurrentSection - 1];

        public int CurrentSection { get; private set; }
        public int CurrentHeight => heightsOfSections[CurrentSection];
        public List<int> HeightsOfSections { get; private set; }

        private void Start()
        {
            //TODO: playerloaded set lele no screen shake
            //SceneManager.Instance.OnPlayersLoaded += SetLevelNoScreenshake;
            HeightsOfSections = heightsOfSections;
        }

        /// <summary>Move flood to new level</summary>
        private void SetLevel()
        {
            ScreenShake.Instance.StartCoroutine(ScreenShake.Instance.Shake(screenShakeDuration, screenShakeIntensity,
                new Vector2(transform.position.x, transform.position.z)));
            SetLevelNoScreenshake();
            boxCollider.size += new Vector3(0, heightsOfSections[CurrentSection], 0);
            boxCollider.center -= new Vector3(0, heightsOfSections[CurrentSection] / 2f, 0);
            StartCoroutine(AfterFlood());
        }

        /// <summary>Move flood to new level</summary>
        private void SetLevelNoScreenshake()
        {
            Vector3 newPos = new Vector3(transform.localPosition.x, CurrentHeight, transform.localPosition.z);
            LeanTween.moveLocal(gameObject, newPos, durationOfFlood);
        }

        /// <summary>Increment currentSection counter and start flooding the next level</summary>
        public void FloodNextLevel()
        {
            CurrentSection++;

            // Guard clause to exit if there is no next section
            if (CurrentSection > heightsOfSections.Count)
            {
                return;
            }
            SetLevel();
        }

        private IEnumerator AfterFlood()
        {
            yield return new WaitForSeconds(durationOfFlood);
            GameTimeManager.Instance.OnRoundIncrement.Invoke();
            StopCoroutine(AfterFlood());
        }
    }
}