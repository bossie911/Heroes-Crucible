using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStudio.HunterGatherer.Networking;
using DG.Tweening;
using GameStudio.HunterGatherer.Divisions;

namespace GameStudio.HunterGatherer.FogOfWar
{
    /// <summary> Lerps the radius of the FogOfWar to a given radius </summary>
    public class LerpFogOfWarRadius : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float lerpDuration = 5f;

        /// <summary> Sets the range of the FogOfWarMasks of all divisions on certain events in the Day and Night Cycle Manager </summary>
        public void SetFogOfWarRadius(float radius)
        {
            /*List<Transform> masks = new List<Transform>();
            List<Division> affectedDivisions = NetworkingPlayerManager.Instance.IsSpectating ?
                Selection.SelectionManager.Instance.SelectableDivisions :
                Selection.SelectionManager.Instance.SelectableOwnedDivisions;

            foreach (Division division in affectedDivisions)
            {
                masks.Add(division.transform.GetComponentInChildren<FogOfWarMask>().MaskReference.transform);
            }

            foreach (Transform mask in masks)
            {
                mask.DOScale(new Vector3(radius, mask.localScale.y, radius), lerpDuration);
            }*/
        }
    }
}
