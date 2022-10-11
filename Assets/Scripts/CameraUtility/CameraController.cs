using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.GameState;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Selection;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.CameraUtility
{
    /// <summary>Handle camera movement and rotations</summary>
    [RequireComponent(typeof(CameraInput))]
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float minPanSpeed = 0.3f;
        
        [SerializeField]
        private float maxPanSpeed = 1.5f;

        [SerializeField]
        private float panSpeedMultiplier = 5;

        [SerializeField]
        private float camDistanceMin = 10f;

        [SerializeField]
        private float camDistanceMax = 50f;

        [SerializeField]
        private float camDistanceDefault = 30f;

        [SerializeField]
        private float camDistanceDelta = 3f;

        [SerializeField]
        private float camHeightMin = 10f;

        [SerializeField]
        private float camTiltMin = 30f;

        [SerializeField]
        private float camTiltMax = 60f;

        [SerializeField]
        private float camTiltDefault = 50f;

        [SerializeField]
        private float camMouseTiltSensitivity = 2f;

        [SerializeField]
        private float camRotationDelta = 2f;

        [SerializeField]
        private float camMouseRotationSensitivity = 2f;

        [Header("References")]
        [SerializeField]
        private Camera cam = null;

        private float camDistance = 0f;
        private float camAngle = 0f;
        private float camRotation = 0f;
        private CameraInput input;

        public float CamDistanceMin
        {
            get { return camDistanceMin; }
        }

        public float CamDistanceMax
        {
            get { return camDistanceMax; }
        }

        public float CamDistance
        {
            get { return camDistance; }
        }

        private void Start()
        {
            camDistance = camDistanceDefault;
            camAngle = camTiltDefault;
            input = GetComponent<CameraInput>();
        }

        private void FixedUpdate()
        {
            Rotate(input.Rotation * camRotationDelta);

            Pan(input.PanDirection);
        }

        private void Update()
        {
            Scroll(input.ScrollDelta * camDistanceDelta);

            Rotate(input.MouseRotation * camMouseRotationSensitivity);

            Tilt(input.MouseTilt * camMouseTiltSensitivity);

            ResetPosition();

            UpdatePosition();
        }

        /// <summary>Update the camera height to move with the terrain and the flood</summary>
        private void ClampToTerrainAndFlood()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 100, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, LayerMask.GetMask("Terrain", "Flood")))
            {
                Vector3 intersectionWithTerrain = hit.point;
                transform.position = intersectionWithTerrain;
            }
        }

        /// <summary>Update the camera position and orientation based on angle and distance, clamped to a minimum height</summary>
        private void UpdatePosition()
        {
            Vector3 camPos = new Vector3(0f, Mathf.Sin(camAngle * Mathf.PI / 180) * camDistance, -(Mathf.Cos(camAngle * Mathf.PI / 180) * camDistance));
            cam.transform.localPosition = camPos;
            cam.transform.LookAt(transform);

            // Raise camera position if position is not at minimum height above terrain and flood
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position + Vector3.up * 100, Vector3.down, out hit, camHeightMin + 100, LayerMask.GetMask("Terrain", "Flood")))
            {
                Vector3 intersectionWithTerrain = hit.point;
                camPos = intersectionWithTerrain + Vector3.up * camHeightMin;
                cam.transform.position = camPos;
            }
        }

        /// <summary>Change camera distance from centerpoint with given delta</summary>
        private void Scroll(float distanceDelta)
        {
            camDistance += distanceDelta;
            camDistance = Mathf.Clamp(camDistance, camDistanceMin, camDistanceMax);
        }

        /// <summary>Change camera rotation around centerpoint with given delta</summary>
        private void Rotate(float rotationDelta)
        {
            camRotation += rotationDelta;
            transform.rotation = Quaternion.Euler(0f, camRotation, 0f);
        }

        /// <summary>Change camera tilt (= angle between centerpoint and camera) with given delta </summary>
        private void Tilt(float angleDelta)
        {
            camAngle += angleDelta;
            camAngle = Mathf.Clamp(camAngle, camTiltMin, camTiltMax);
        }

        /// <summary>Move camera centerpoint position in given direction</summary>
        private void Pan(Vector3 direction)
        {
            float zoom = (camDistance - camDistanceMin) / (camDistanceMax - camDistanceMin);
            float speed = Mathf.Lerp(minPanSpeed, maxPanSpeed, zoom) * (input.PanBoost ? panSpeedMultiplier : 1);
            transform.Translate(direction * speed, Space.Self);
            ClampToTerrainAndFlood();
        }

        /// <summary>Sets the camera position to the nearest selected obejct. If none are selected go to nearest unit</summary>
        private void ResetPosition()
        {
            if (!input.GoToNearestUnit || GameStateManager.Instance.CurrentState == PlayerState.Spectating)
            {
                return;
            }

            if (GameStateManager.Instance.CurrentState != PlayerState.Lost)
            {
                int selectedCount = SelectionManager.Instance.SelectedObjects.Count;

                switch (selectedCount)
                {
                    case 0:
                        {
                            // Guard clause to exit if we don't have any divisions
                            if (SelectionManager.Instance.SelectableOwnedDivisions.Count == 0)
                            {
                                return;
                            }

                            //Go to nearest unit
                            transform.position = GetClosestTransform(SelectionManager.Instance.SelectableOwnedDivisions).position;
                            break;
                        }
                    case 1:
                        {
                            //Go to selected object
                            transform.position = SelectionManager.Instance.SelectedObjects[0].transform.position;
                            break;
                        }
                    default:
                        {
                            //Go to nearest selected object
                            transform.position = GetClosestTransform(SelectionManager.Instance.SelectedObjects).position;
                            break;
                        }
                }
            }

            ClampToTerrainAndFlood();
        }

        /// <summary>Returns the closest Transform of the transforms given</summary>
        private Transform GetClosestTransform(List<Transform> transforms)
        {
            Transform closestTransform = transforms[0];
            float closestDistance = (transform.position - transforms[0].position).sqrMagnitude;

            for (int i = 1; i < transforms.Count; i++)
            {
                float distance = (transform.position - transforms[i].position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestTransform = transforms[i];
                    closestDistance = distance;
                }
            }

            return closestTransform;
        }

        /// <summary>Returns the closest Transform of the SelectableObjects given</summary>
        private Transform GetClosestTransform(List<SelectableObject> objects)
        {
            List<Transform> transforms = new List<Transform>();

            foreach (SelectableObject obj in objects)
            {
                transforms.Add(obj.transform);
            }

            return GetClosestTransform(transforms);
        }

        /// <summary>Returns the closest Transform of the NetworkedMovingObjects given</summary>
        private Transform GetClosestTransform(List<Division> divisions)
        {
            List<Transform> transforms = new List<Transform>();

            foreach (Division division in divisions)
            {
                transforms.Add(division.transform);
            }
            return GetClosestTransform(transforms);
        }

        /// <summary>Set position externally to given position</summary>
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        ///<summary>Set position externally to given position and reset the y value</summary>
        public void SetPosition(Vector2 position)
        {
            Vector3 newPosition = new Vector3(position.x, 0, position.y);

            transform.position = newPosition;
            ClampToTerrainAndFlood();
        }
    }
}