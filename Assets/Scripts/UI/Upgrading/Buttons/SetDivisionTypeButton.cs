using System.Collections;
using System.Collections.Generic;
using GameStudio.HunterGatherer.Divisions;
using UnityEngine;

namespace GameStudio.HunterGatherer.UI
{
    public class SetDivisionTypeButton : ActionButton
    {
        public Division Division { get; set; }
        public DivisionType type { get; set; }

        public override void PerformAction()
        {
            Division.SetType(type);
            onPerformAction?.Invoke();
        }

        public override void Show(Object obj)
        {
            Division = obj as Division;
            imgIcon.sprite = Division.DivisionTypeDatas.GetDivisionTypeData(type).Icon;
            gameObject.SetActive(true);
        }
    }
}