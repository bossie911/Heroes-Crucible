using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Divisions.Upgrades;
using UnityEngine;

namespace GameStudio.HunterGatherer.UI
{
    public class SetDivisionTypeCarousel : ButtonCarousel
    {
        [Header("Type Reference(s)")]
        [SerializeField] DivisionTypeDatas divisionTypeDatas;
        [SerializeField] int startOffset = 2;

        protected override void GeneratePool()
        {
            actionPool = new List<ActionButton>();
            for (int i = startOffset; i < divisionTypeDatas.DivisionCount; i++)
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

            SetCircle(actionPool.Count);

            gameObject.SetActive(true);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            transform.position = (sender as Division).transform.position;
        }

        public override void Show(Object sender, Vector3 position, List<Object> objects)
        {
            this.sender = sender;
            transform.position = position;

            for (int i = 0; i < actionPool.Count; i++)
            {
                if (i < divisionTypeDatas.DivisionCount)
                {
                    (actionPool[i] as SetDivisionTypeButton).type = divisionTypeDatas.GetDivisionTypeData(i + startOffset).Type;
                    actionPool[i].Show(objects[0]);
                    actionPool[i].onPerformAction += DeActivateAll;
                }
                else
                    actionPool[i].gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
        }
    }
}