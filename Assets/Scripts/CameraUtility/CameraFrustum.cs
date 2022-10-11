using UnityEngine;

namespace GameStudio.HunterGatherer.CameraUtility
{
    /// <summary>Create a frustum with a Vector3 array and remember the information</summary>
    public class CameraFrustum : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float terrainHeight = 0;

        [Header("References")]
        [SerializeField]
        private Camera playerCamera = null;

        private Vector3[] projectorCorners = new Vector3[4];
        private float cameraHeight;
        private RaycastHit hit;

        public Vector3 Center { get; private set; }
        public float TerrainHeight { get; }

        private void Awake()
        {
            if (playerCamera == null)
            {
                Debug.LogError("The variable playerCamera in FrustumGenerator doesn't have a value assigned!");
            }
        }

        /// <summary>Create a schematic or realistic frustum depending on the settings</summary>
        public Vector3[] GetFrustumCorners(float height, CameraFrustumType frustumType)
        {
            switch (frustumType)
            {
                case CameraFrustumType.Schematic:
                    GetFrustumCornersSchematic(height);
                    break;
                case CameraFrustumType.Realistic:
                    GetFrustumCornersRealistic(height);
                    break;
            }

            return this.projectorCorners;
        }

        ///<summary>Update the vectors of the frustum's corners and center in a schematic way</summary>
        public Vector3[] GetFrustumCornersSchematic(float height)
        {
            projectorCorners[0] = GetPointAtHeight(playerCamera.ViewportPointToRay(new Vector3(0, 0, 0)), height);
            projectorCorners[1] = GetPointAtHeight(playerCamera.ViewportPointToRay(new Vector3(0, 1, 0)), height);
            projectorCorners[2] = GetPointAtHeight(playerCamera.ViewportPointToRay(new Vector3(1, 1, 0)), height);
            projectorCorners[3] = GetPointAtHeight(playerCamera.ViewportPointToRay(new Vector3(1, 0, 0)), height);

            Center = GetPointAtHeight(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), height);

            return projectorCorners;
        }

        ///<summary>Update the vectors of the frustum's corners and center in a realistic way</summary>
        public Vector3[] GetFrustumCornersRealistic(float height)
        {
            projectorCorners[0] = GetPointAtHit(playerCamera.ViewportPointToRay(new Vector3(0, 0, 0)), height);
            projectorCorners[1] = GetPointAtHit(playerCamera.ViewportPointToRay(new Vector3(0, 1, 0)), height);
            projectorCorners[2] = GetPointAtHit(playerCamera.ViewportPointToRay(new Vector3(1, 1, 0)), height);
            projectorCorners[3] = GetPointAtHit(playerCamera.ViewportPointToRay(new Vector3(1, 0, 0)), height);

            Center = GetPointAtHeight(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), height);

            return projectorCorners;
        }

        /// <summary>Return the intersection position of a ray and a plane (height), clamped to the farplane.</summary>
        private Vector3 GetPointAtHeight(Ray ray, float height)
        {
            Vector3 vectorToPlane = (ray.origin.y - height) / -ray.direction.y * ray.direction;
            float vectorToPlaneMagnitude = vectorToPlane.magnitude;

            // Clamp vectorToPlane if beyond the far plane
            if (float.IsNaN(vectorToPlaneMagnitude) || vectorToPlaneMagnitude > playerCamera.farClipPlane)
            {
                vectorToPlane = ray.direction * playerCamera.farClipPlane;
            }

            Vector3 output = ray.origin + vectorToPlane;
            output.y = terrainHeight;

            return output;
        }

        /// <summary>Return the intersection position of a ray and the plane of where the ray hit</summary>
        private Vector3 GetPointAtHit(Ray ray, float height)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, 1 << 8))
            {
                return hit.point;
            }
            else
            {
                return GetPointAtHeight(ray, height);
            }
        }
    }
}