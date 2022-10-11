using GameStudio.HunterGatherer.CameraUtility;
using GameStudio.HunterGatherer.GameTime;
using GameStudio.HunterGatherer.Selection;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    public class References : MonoBehaviour
    {
        private static References instance;
        
        [SerializeField] public SelectionManager SelectionManager;
        [SerializeField] public CameraInput CameraInput;
        [SerializeField] public CameraController cameraController;
        [SerializeField] public GameTimeManager GameTimeManager;

        public static References Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = GameObject.FindObjectOfType<References>();
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            else
            {
                instance = this;
            }
        }
    }
}


