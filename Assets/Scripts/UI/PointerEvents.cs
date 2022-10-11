using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameStudio.HunterGatherer.UI
{
    /// <summary>Monobehaviour that can be attached to objects to make pointer events listenable in the inspector</summary>
    public class PointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
        public UnityEvent OnClick;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExit.Invoke();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
        }
    }
}