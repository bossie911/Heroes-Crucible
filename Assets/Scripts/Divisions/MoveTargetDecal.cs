using System.Collections;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary>Class used to make a projector show a tag when giving a move order</summary>
    public class MoveTargetDecal : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float disappearTime = 1f;

        [SerializeField]
        private float sizeToDisapearOn = 0.1f;

        [Header("References")]
        [SerializeField]
        private Projector projector = null;

        private float defaultOrthographicSize;

        private void Awake()
        {
            defaultOrthographicSize = projector.orthographicSize;
        }

        /// <summary>Places this object on given position and initializes it</summary>
        public void PlaceOnPosition(Vector3 position)
        {
            //mvoe to position
            transform.position = position;

            //stop the DisappearEnumerator if it is already running
            StopCoroutine(DisappearEnumerator());

            //set projecter to default size and color
            projector.orthographicSize = defaultOrthographicSize;

            //start a new DisappearEnumerator to make it disappear
            StartCoroutine(DisappearEnumerator());
        }

        /// <summary>Reduces orthograpic size until it can disappear</summary>
        private IEnumerator DisappearEnumerator()
        {
            float currentSizeTime = 0;

            while (currentSizeTime < disappearTime)
            {
                currentSizeTime += Time.deltaTime;
                if (currentSizeTime > disappearTime)
                {
                    currentSizeTime = disappearTime;
                }

                projector.orthographicSize = Mathf.Lerp(defaultOrthographicSize, sizeToDisapearOn, currentSizeTime / disappearTime);

                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}