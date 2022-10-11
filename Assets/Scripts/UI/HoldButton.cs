using System.Collections;

using GameStudio.HunterGatherer.CustomEvents;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.UI
{
    /// <summary> Custom override of the default Unity button. Allows the same functionality as Button, and includes the HoldDown option. </summary>
    public class HoldButton : Button
    {
        [SerializeField]
        private float holdDownDuration = 1f;

        [SerializeField]
        private UnityEventFloat onHeldDown = new UnityEventFloat();

        [SerializeField]
        private UnityEvent onHeldDownFinished = new UnityEvent();

        private bool isShortClick;
        private bool isPointerHeldDown;
        private float heldDownDuration;

        public float HeldDownDuration
        {
            get
            {
                return heldDownDuration;
            }
            set
            {
                heldDownDuration = value;
                onHeldDown?.Invoke(value / holdDownDuration);
            }
        }

        public float HoldDownDuration
        {
            get
            {
                return holdDownDuration;
            }
            set
            {
                holdDownDuration = value;
            }
        }

        protected override void OnDisable() => HeldDownDuration = 0f;

        /// <summary> 
        /// OnHeldDown an event that is called every frame that the button is being held down. 
        /// Param provided by the event is the percentage (0.0 - 1.0) to how close it is to calling OnHeldDownFinished. 
        /// </summary>
        public UnityEventFloat OnHeldDown { get => onHeldDown; }
        /// <summary> OnHeldDownFinished is an event that is called once holdDownDuration has been met. </summary>
        public UnityEvent OnHeldDownFinished { get => onHeldDownFinished; }
        public bool EnableHoldDown { get; set; } = true;

        /// <summary>  Override default unity OnPointerClick method </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (isShortClick)
            {
                base.OnPointerClick(eventData);
            }
        }

        /// <summary>  Override default unity OnPointerDown method </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (EnableHoldDown)
            {
                isPointerHeldDown = true;
                StartCoroutine(Countdown());
            }
        }

        /// <summary>  Override default unity OnPointerUp method </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            isShortClick = (HeldDownDuration <= 0.2f);
            isPointerHeldDown = false;
            HeldDownDuration = 0f;
        }

        /// <summary> Coroutine that checks if this button has been held down for a certain amount of time. </summary>
        public IEnumerator Countdown()
        {
            while (isPointerHeldDown && HeldDownDuration < holdDownDuration)
            {
                HeldDownDuration += Time.unscaledDeltaTime;

                if (HeldDownDuration >= holdDownDuration)
                {
                    onHeldDownFinished?.Invoke();
                }
                yield return null;
            }
        }
    }
}
