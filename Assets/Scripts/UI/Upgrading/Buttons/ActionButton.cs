using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer.Divisions;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.UI
{
    [RequireComponent(typeof(Button)), RequireComponent(typeof(Image))]
    public abstract class ActionButton : MonoBehaviour
    {
        protected Sprite icon;
        public Sprite Icon
        {
            get => icon;
            protected set
            {
                icon = value;
                imgIcon.sprite = icon;
            }
        }

        protected Button btn;

        [SerializeField]
        protected Image imgIcon;
        [SerializeField]
        protected Image imgBackground;

        bool contains;

        public System.Action<ActionButton> onClick;
        public System.Action onPerformAction;
        public System.Action onDeactivate;

        protected virtual void Awake()
        {
            if (!btn)
            {
                btn = GetComponent<Button>();
                if (!btn)
                    btn = gameObject.AddComponent<Button>();
            }

            if (!imgIcon)
            {
                imgIcon = GetComponentInChildren<Image>();
                if (!imgIcon)
                    imgIcon = gameObject.AddComponent<Image>();
            }

            if (!imgBackground)
            {
                imgBackground = GetComponent<Image>();
                if (!imgBackground)
                    imgBackground = gameObject.AddComponent<Image>();
            }

            btn.onClick.AddListener(OnClick);
            btn.onClick.AddListener(PerformAction);

            gameObject.SetActive(false);
        }

        protected virtual void OnClick()
        {
            onClick?.Invoke(this);
        }

        public abstract void Show(Object obj);
        public abstract void PerformAction();
        public virtual void Deactivate()
        {
            onDeactivate?.Invoke();
        }
    }
}