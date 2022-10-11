using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Divisions.Upgrades;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace GameStudio.HunterGatherer.UI
{
    public abstract class ButtonCarousel : MonoBehaviour
    {
        [Header("Object Reference")]
        [SerializeField] protected GameObject actionButton;

        [Header("Pooling")]
        [SerializeField] private int poolSize = 7;
        protected List<ActionButton> actionPool;

        [Header("Circle Settings")]
        [SerializeField] float radius;
        [SerializeField] float minButtonSize, maxButtonSize;
        [SerializeField] float scaleDuration;
        protected ActionButton focussedButton;

        public System.Action<ButtonCarousel> OnDeActivate;

        protected Object sender;

        protected virtual void Awake()
        {
            GeneratePool();
            GetComponent<Canvas>().worldCamera = Camera.main;
            gameObject.SetActive(false);
        }

        /// <summary>Generate a pool of buttons</summary>
        protected virtual void GeneratePool()
        {
            actionPool = new List<ActionButton>();
            for (int i = 0; i < poolSize; i++)
            {
                try
                {
                    actionPool.Add(Instantiate(actionButton, transform).GetComponent<ActionButton>());
                }
                catch (MissingComponentException)
                {
                    Debug.LogError($"<b>[{gameObject.name}]</b> Your actionButton prefab doesn't contain the ActionButton component!");
                }
            }
        }

        protected virtual void FixedUpdate()
        {
        }


        protected void OnFocus(ActionButton sender)
        {
            if (focussedButton)
                UpgradeUIManager.Instance.DeActivate(focussedButton);

            focussedButton = sender;
            ResetButtonScale();

            for (int i = 0; i < actionPool.Count; i++)
            {
                if (actionPool[i] != focussedButton)
                {
                    actionPool[i].transform.DOScale(minButtonSize, scaleDuration);
                }
            }
        }

        protected void ResetButtonScale()
        {
            for (int i = 0; i < actionPool.Count; i++)
            {
                actionPool[i].transform.DOScale(maxButtonSize, scaleDuration);
            }
        }

        protected void ResetButtons()
        {
            for (int i = 0; i < actionPool.Count; i++)
            {
                actionPool[i].onDeactivate -= DeActivateAll;
                actionPool[i].onClick -= OnFocus;
                actionPool[i].onPerformAction -= actionPool[i].Deactivate;

                try
                {
                    actionPool[i].Deactivate();
                    actionPool[i].gameObject.SetActive(false);
                }
                catch (MissingReferenceException) { actionPool[i].Deactivate();}
            }

            ResetButtonScale();
        }

        /// <summary>Show the UI</summary>
        /// <param name="position">The worldspace at which to show the UI</param>
        /// <param name="objects">List of objects referenced in the UI</param>
        public virtual void Show(Object sender, Vector3 position, List<Object> objects)
        {
            this.sender = sender;
            transform.position = position;

            for (int i = 0; i < actionPool.Count; i++)
            {
                if (i < objects.Count)
                {
                    actionPool[i].Show(objects[i]);
                    actionPool[i].onClick += OnFocus;
                    actionPool[i].onDeactivate += DeActivateAll;
                    actionPool[i].onPerformAction += actionPool[i].Deactivate;
                }
                else
                    actionPool[i].gameObject.SetActive(false);
            }

            SetCircle(objects.Count);

            gameObject.SetActive(true);
        }

        public void SetCircle(int nButtons)
        {
            Vector3 tempPos = Vector3.zero;
            for (int i = 0; i < nButtons; i++)
            {
                tempPos.x = radius * Mathf.Sin((i) / (nButtons / 2f) * Mathf.PI);
                tempPos.z = 0;
                tempPos.y = radius * Mathf.Cos((i) / (nButtons / 2f) * Mathf.PI);
                actionPool[i].transform.localPosition = tempPos;
            }
        }

        public virtual void DeActivate()
        {
            ResetButtons();

            OnDeActivate?.Invoke(this);
            gameObject.SetActive(false);
        }


        public virtual void DeActivateAll()
        {
            ResetButtons();

            if (sender.GetType() == typeof(ButtonCarousel))
            {
                (sender as ButtonCarousel).DeActivateAll();
            }
            if (sender.GetType().IsSubclassOf(typeof(ActionButton)))
            {
                (sender as ActionButton).Deactivate();
            }

            OnDeActivate?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}