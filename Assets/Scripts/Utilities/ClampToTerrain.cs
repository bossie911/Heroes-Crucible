using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Utility for clamping objects to terrain in the editor (e.g. spawnpoints)</summary>
    [ExecuteInEditMode]
    public class ClampToTerrain : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("Use clamp y position to terrain underneath")]
        private bool clamp = false;

        [SerializeField]
        private float offset = 25f;

        private void Update()
        {
            if (!clamp)
            {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, offset, 0), Vector3.down, out hit, 1000f, LayerMask.GetMask("Terrain")))
            {
                transform.position = hit.point;
            }
            else
            {
                Debug.LogWarning("Can't clamp object to train :: No terrain is under " + name);
            }
        }
    }
}