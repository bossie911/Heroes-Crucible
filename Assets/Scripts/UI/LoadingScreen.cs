using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameStudio.HunterGatherer.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Image Variables")]
        [SerializeField] bool animateImage = true;
        [SerializeField] Sprite loadingImage;
        [SerializeField] Color loadingImageColor = Color.white;
        [SerializeField] float baseRotationSpeed;
        [SerializeField] AnimationCurve rotationSpeedCurve;

        [Header("Text Variables")]
        [SerializeField] bool animateText;
        [SerializeField] string loadingTextSyntax = "Loading[char]";
        [SerializeField] string loadingTextPlaceholder = "[char]";
        [SerializeField] string loadingChar = ".";
        [SerializeField] int maxLength = 3;
        [SerializeField] int updateInterval = 60;
        int updates;
        [SerializeField] Color loadingTextColor = Color.white;

        Regex regex;

        [Header("Object References")]
        [SerializeField] Image loadingImageObject;
        [SerializeField] TextMeshProUGUI loadingTextObject;

        new Coroutine animation;

        static LoadingScreen instance;
        public static LoadingScreen Instance => instance;

        void OnValidate()
        {
            if (loadingImageObject)
            {
                loadingImageObject.sprite = loadingImage;
                loadingImageObject.color = loadingImageColor;
            }

            if (loadingTextObject)
            {
                loadingTextObject.color = loadingTextColor;
            }

            if (loadingChar.Length > 1)
            {
                loadingChar = loadingChar[0].ToString();
            }
        }

        void Awake()
        {
            if (instance != null)
            {
                gameObject.SetActive(false);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);

            regex = new Regex(@$"\{loadingChar}+");
        }

        IEnumerator Animate()
        {
            while (true)
            {
                if (animateImage)
                {
                    loadingImageObject.transform.Rotate(-baseRotationSpeed * rotationSpeedCurve.Evaluate(loadingImageObject.transform.localRotation.z / 360f) * Vector3.forward, Space.Self);
                }
                if (animateText && updates % updateInterval == 0)
                {
                    if (regex.Match(loadingTextObject.text).Value.Length < maxLength)
                    {
                        loadingTextObject.text += loadingChar;
                    }
                    else
                    {
                        loadingTextObject.text = loadingTextSyntax.Replace(loadingTextPlaceholder, loadingChar);
                    }
                }

                updates++;
                yield return null;
            }
        }

        public void Play()
        {
            gameObject.SetActive(true);
            Stop();
            animation = StartCoroutine(Animate());
        }

        public void Stop(bool disableGameObject = false)
        {
            if (animation != null)
                StopCoroutine(animation);

            if (disableGameObject)
                gameObject.SetActive(false);
        }

        void OnDisable()
        {
            Stop();
        }
    }
}