using UnityEngine;
using UnityEngine.EventSystems;
using GameStudio.HunterGatherer.CameraUtility;

namespace GameStudio.HunterGatherer.Minimap
{
    ///<summary>Handle input on minimap and change the camera's position</summary>
    public class MinimapInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Settings")]
        [SerializeField]
        [Range(0.01f, 1)]
        private float cameraFollowSpeed = 0.5f;

        [Header("References")]
        [SerializeField]
        private CameraController playerCameraController = null;

        private RectTransform minimapTransform;
        private MinimapReferences references;
        private Vector2 desiredPosition;
        private bool isFollowing = false;

        private void Awake()
        {
            if (playerCameraController == null)
            {
                Debug.LogError("The variable playerCamera in MinimapInput doesn't have a value assigned!");
            }

            minimapTransform = GetComponent<RectTransform>();
            references = GetComponent<MinimapReferences>();
        }

        private void Update()
        {
            if (isFollowing)
            {
                Vector2 translateVector = new Vector2(
                    desiredPosition.x - playerCameraController.transform.position.x,
                    desiredPosition.y - playerCameraController.transform.position.z);

                playerCameraController.SetPosition(desiredPosition - translateVector * cameraFollowSpeed);
            }
        }

        /// <summary>Move the camera of the player depending on where the minimap was clicked</summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            isFollowing = true;

            Vector2 input = Vector2.zero;

            // Check if the minimap was clicked
            if (eventData.button != PointerEventData.InputButton.Left || 
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    minimapTransform, eventData.position, eventData.pressEventCamera, out input))
            {
                return;
            }

            input = MinimapToWorldspace(input, false);

            playerCameraController.SetPosition(input);
            desiredPosition = input;
        }

        /// <summary>Order the player camera to stop following the desired position</summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            isFollowing = false;
        }

        /// <summary>Change the desired position of the camera where the minimap was dragged</summary>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 input = Vector2.zero;

            // Check if the minimap was clicked
            if (eventData.button != PointerEventData.InputButton.Left || 
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    minimapTransform, eventData.position, eventData.pressEventCamera, out input))
            {
                return;
            }

            input = MinimapToWorldspace(input, true);

            desiredPosition = input;
        }

        /// <summary>Get the position of the world from where the minimap was clicked</summary>
        private Vector2 MinimapToWorldspace(Vector2 input, bool shouldClamp)
        {
            // Calculate the position the camera needs to go and move it
            input = input * 2 / minimapTransform.sizeDelta;

            input += minimapTransform.pivot * 2;

            input.x -= 1;
            input.y -= 1;

            input *= references.DepthCamera.orthographicSize;

            if (shouldClamp)
            {
                input.x = Mathf.Clamp(input.x, -references.DepthCamera.orthographicSize, references.DepthCamera.orthographicSize);
                input.y = Mathf.Clamp(input.y, -references.DepthCamera.orthographicSize, references.DepthCamera.orthographicSize);
            }

            input.x += references.WorldCenterTransform.transform.position.x;
            input.y += references.WorldCenterTransform.transform.position.z;

            return input;
        }
    }
}