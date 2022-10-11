using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary> Attach this script to an object to make it look at the camera </summary>
    public class LookAtCamera : MonoBehaviour
    {
        private Camera cam;

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            if (!NetworkClient.active) return;
            LookAtCameraBillBoard();
            ScaleWithCameraHeight();
        }
        private void ScaleWithCameraHeight()
        {
            transform.localScale = new Vector3(1, 1, 1) * Camera.main.transform.position.y * 0.008f;
        }
        /// <summary> Look at the camera billboard </summary>
        private void LookAtCameraBillBoard()
        {
            if(cam is null)
            {
                cam = GameObject.Find("Main Camera").GetComponent<Camera>();
                if(cam is null)
                {
                    Debug.LogError("fuck");
                    return;
                }
            }
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}
