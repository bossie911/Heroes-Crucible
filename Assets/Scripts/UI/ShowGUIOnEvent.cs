using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStudio.HunterGatherer;
using GameStudio.HunterGatherer.Divisions.UI;

namespace GameStudio.HunterGatherer.UI
{
    public class ShowGUIOnEvent : MonoBehaviour
    {
        [SerializeField] bool hideOnStart = false;
        [SerializeField] string onShowGUI;

        protected virtual void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(!hideOnStart);
            EventManager.Instance.AddEvent(onShowGUI, true);
            EventManager.Instance.AddListener(onShowGUI, ShowGUI);
        }

        protected virtual void OnDestroy()
        {
            EventManager.Instance.RemoveListener(onShowGUI, ShowGUI);
        }

        protected virtual void ShowGUI(System.EventArgs divisionArgs)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}