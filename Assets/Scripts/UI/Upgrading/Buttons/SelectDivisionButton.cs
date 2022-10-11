using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameStudio.HunterGatherer.Divisions;

namespace GameStudio.HunterGatherer.UI
{
    public class SelectDivisionButton : ActionButton
    {
        Division _division;
        public Division division
        {
            get => _division;
            protected set
            {
                _division = value;
                if (_division != null)
                {
                    imgIcon.sprite = division.TypeData.Icon;
                    imgBackground.color = division.DivisionColor;
                }
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            UpgradeUIManager.Instance.Show(this, transform.position, new List<Object> { division });
        }


        public override void PerformAction()
        {

        }

        public override void Show(Object obj)
        {
            this.division = obj as Division;
            gameObject.SetActive(true);
        }
    }
}