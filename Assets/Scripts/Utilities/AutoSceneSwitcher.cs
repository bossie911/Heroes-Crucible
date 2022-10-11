using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStudio.HunterGatherer.Utilities
{
    public class AutoSceneSwitcher : MonoBehaviour
    {
        [SerializeField] int mainSceneBuildIndex = 0;
        void Awake()
        {
            /*if (SceneManager.GetActiveScene().buildIndex != mainSceneBuildIndex)
                SceneManager.LoadScene(mainSceneBuildIndex, LoadSceneMode.Single);*/
        }
    }
}