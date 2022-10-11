using GameStudio.HunterGatherer.GameSettings;
using UnityEngine;

namespace GameStudio.HunterGatherer.CameraUtility
{
    /// <summary>Handle camera input</summary>
    public class CameraInput : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private bool enableArrowsPanning = true;

        [SerializeField]
        private bool enableWASDPanning = true;

        [SerializeField]
        private bool enableMousePanning = true;

        [SerializeField]
        private bool enableEdgePanning = false;

        [SerializeField]
        private bool enableEdgePanningInEditor = false;

        [SerializeField]
        private bool enableQERotation = true;

        [SerializeField]
        private bool enableMouse3Rotation = true;

        [SerializeField]
        private bool enableMouse3Tilt = true;

        [SerializeField]
        private bool invertQERotation = false;

        [SerializeField]
        private bool invertMouse3Rotation = false;

        [SerializeField]
        private bool invertMouse3Tilt = false;

        [SerializeField]
        private float panBorderThickness = 10f;

        [Header("References")]
        [SerializeField]
        private Camera cam = null;

        private bool gameHasFocus = true;
        private bool gameIsPaused = false;

        public Vector3 PanDirection { get; private set; }
        public bool PanBoost { get; private set; }
        public int ScrollDelta { get; private set; }
        public float Rotation { get; private set; }
        public float MouseRotation { get; private set; }
        public float MouseTilt { get; private set; }
        public bool GoToNearestUnit { get; private set; }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            gameHasFocus = true;
            GameObject.Find("EscMenu").GetComponent<EscMenu>().OnToggleMenu.AddListener(OnEscapeMenu);
        }

        private void Update()
        {
            if (gameHasFocus && !gameIsPaused)
            {
                PanDirection = GetPanDirection();
            }
            else
            {
                PanDirection = Vector3.zero;
            }

            PanBoost = Input.GetKey(KeyCode.LeftShift);

            ScrollDelta = GetScrollDelta();

            Rotation = GetRotation();

            MouseRotation = GetMouseRotation();

            MouseTilt = GetMouseTilt();

            GoToNearestUnit = Input.GetKey(KeyCode.Space);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            gameHasFocus = hasFocus;
        }

        private void OnEscapeMenu()
        {
            gameIsPaused = !gameIsPaused;
        }

        /// <summary>Return the panning direction based on enabled input</summary>
        private Vector3 GetPanDirection()
        {
            Vector3 panDirection = Vector3.zero;

            if ((enableArrowsPanning && Input.GetKey(KeyCode.RightArrow)) || (enableWASDPanning && Input.GetKey(KeyCode.D)) || (enableMousePanning && Input.mousePosition.x >= Screen.width - panBorderThickness))
            {
                panDirection += Vector3.right;
            }

            if ((enableArrowsPanning && Input.GetKey(KeyCode.LeftArrow)) || (enableWASDPanning && Input.GetKey(KeyCode.A)) || (enableMousePanning && Input.mousePosition.x <= panBorderThickness))
            {
                panDirection += Vector3.left;
            }

            if ((enableArrowsPanning && Input.GetKey(KeyCode.UpArrow)) || (enableWASDPanning && Input.GetKey(KeyCode.W)) || (enableMousePanning && Input.mousePosition.y >= Screen.height - panBorderThickness))
            {
                panDirection += Vector3.forward;
            }

            if ((enableArrowsPanning && Input.GetKey(KeyCode.DownArrow)) || (enableWASDPanning && Input.GetKey(KeyCode.S)) || (enableMousePanning && Input.mousePosition.y <= panBorderThickness))
            {
                panDirection += Vector3.back;
            }

            if (Application.isEditor ? enableEdgePanningInEditor : enableEdgePanning)
            {
                if (cam.ScreenToViewportPoint(Input.mousePosition).x > 1)
                {
                    panDirection += Vector3.right;
                }

                if (cam.ScreenToViewportPoint(Input.mousePosition).x < 0)
                {
                    panDirection += Vector3.left;
                }

                if (cam.ScreenToViewportPoint(Input.mousePosition).y > 1)
                {
                    panDirection += Vector3.forward;
                }

                if (cam.ScreenToViewportPoint(Input.mousePosition).y < 0)
                {
                    panDirection += Vector3.back;
                }
            }

            return panDirection;
        }

        /// <summary>Return the scroll delta input</summary>
        private int GetScrollDelta()
        {
            int scrollDelta = 0;

            if (Input.mouseScrollDelta.y > 0)
            {
                scrollDelta = -1;
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                scrollDelta = 1;
            }

            return scrollDelta;
        }

        /// <summary>Return the rotation input</summary>
        private float GetRotation()
        {
            float rotation = 0;

            if (enableQERotation)
            {
                if (Input.GetKey(KeyCode.Q))
                {
                    rotation += (invertQERotation ? -1 : 1);
                }

                if (Input.GetKey(KeyCode.E))
                {
                    rotation += (invertQERotation ? 1 : -1);
                }
            }

            return rotation;
        }

        /// <summary>Return the mouse rotation input</summary>
        private float GetMouseRotation()
        {
            float mouseRotation = 0;

            if (enableMouse3Rotation)
            {
                if (Input.GetMouseButton(2))
                {
                    mouseRotation += Input.GetAxis("Mouse X") * (invertMouse3Rotation ? -1 : 1);
                }
            }

            return mouseRotation;
        }

        /// <summary>Return the mouse tilt input</summary>
        private float GetMouseTilt()
        {
            float mouseTilt = 0;

            if (enableMouse3Tilt)
            {
                if (Input.GetMouseButton(2))
                {
                    mouseTilt += Input.GetAxis("Mouse Y") * (invertMouse3Tilt ? 1 : -1);
                }
            }

            return mouseTilt;
        }
    }
}