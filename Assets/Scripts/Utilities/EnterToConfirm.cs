using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.Utilities
{
    public class EnterToConfirm : MonoBehaviour
    {
        [SerializeField]
        private Button btnToConfirm = null;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                btnToConfirm.onClick.Invoke();
            }
        }
    }
}