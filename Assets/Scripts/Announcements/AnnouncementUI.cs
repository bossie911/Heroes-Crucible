using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace GameStudio.HunterGatherer.UI
{
    /// <summary> This class let you set an announcement and will show it on the GUI </summary>
    public class AnnouncementUI : MonoBehaviour
    {
        public static AnnouncementUI Instance;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("AnnouncementUI instance has already been set, destroying object.");
                Destroy(gameObject);
            }
        }

        [Header("AnnouncementUI Settings")]
        [SerializeField]
        private float showingDuration = 0f;

        [SerializeField]
        private float fadeDuration = 0f;

        [Header("AnnouncementUI References")]
        [SerializeField]
        private TextMeshProUGUI announcementText = null;

        void Awake()
        {
            this.gameObject.SetActive(true);
        }

        /// <summary> This method let you set an new scriptable object announcement </summary>
        public void ShowAnnouncement(Announcement message)
        {
            announcementText.text = message.Text;
            bool active = gameObject.activeInHierarchy;
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
            StartCoroutine(AnnouncementSequence(active));
        }

        /// <summary> This method let you set an new string announcement </summary>
        public void ShowAnnouncement(string message)
        {
            announcementText.text = message;
            bool active = gameObject.activeInHierarchy;
            Debug.Log(active);
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
            StartCoroutine(AnnouncementSequence(active));
        }

        /// <summary> This coroutine will fade in or fade out the text in the announcementUI </summary>
        private IEnumerator DoFade(float start , float end)
        {
            float counter = 0;
            while (counter < fadeDuration)
            {
                counter += Time.unscaledDeltaTime;
                announcementText.alpha = Mathf.Lerp(start, end, counter / fadeDuration);

                yield return null;
            }
        }

        /// <summary> This coroutine will show the text in the announcementUI for a certain amount of time</summary>
        private IEnumerator ShowTextDuration()
        {
            float counter = 0;
            while (counter < showingDuration)
            {
                counter += Time.unscaledDeltaTime;

                yield return null;
            }
        }

        /// <summary> This coroutine will sequence the coroutines needed to fade in, show and fade out the announcement</summary>
        private IEnumerator AnnouncementSequence(bool active)
        {
            yield return StartCoroutine(DoFade(announcementText.alpha, 1));
            yield return StartCoroutine(ShowTextDuration());
            yield return StartCoroutine(DoFade(announcementText.alpha, 0));
            
            gameObject.SetActive(active);
        }
    }
}
