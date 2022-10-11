using System;
using GameStudio.HunterGatherer.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GameStudio.HunterGatherer.Utilities
{
    /// <summary>Show tooltip on pointer enter and hide on pointer exit</summary>
    public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private bool force = false;
        
        [FormerlySerializedAs("text1")] [SerializeField]
        public string Title = string.Empty;

        [FormerlySerializedAs("text2")] [SerializeField]
        public string Description = string.Empty;

        private bool isActive = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.Instance.Setup(Title, Description, force);
            isActive = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.Instance.Disable(force);
            isActive = false;
        }

        private void OnDisable()
        {
            if (isActive)
                Tooltip.Instance.Disable(force);
        }
    }
}