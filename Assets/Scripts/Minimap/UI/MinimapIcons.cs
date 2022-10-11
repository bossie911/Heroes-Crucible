using GameStudio.HunterGatherer.Divisions;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Minimap
{
    ///<summary>Show icons for divisions on the minimap GUI</summary>
    [RequireComponent(typeof(MinimapReferences))]
    public class MinimapIcons : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Texture icon = null;

        [SerializeField]
        private Texture combatIcon = null;

        [SerializeField]
        private int iconSize = 10;

        public List<Division> Divisions { get; private set; } = new List<Division>();

        private RectTransform minimapRectTransform;
        private Rect minimapRect;
        private MinimapReferences references;

        private void Awake()
        {
            minimapRectTransform = GetComponent<RectTransform>();
            references = GetComponent<MinimapReferences>();
        }

        private void OnGUI()
        {
            Vector2 position;
            minimapRect = references.RectTransformToScreenSpace(minimapRectTransform);

            foreach (var division in Divisions)
            {
                if (division != null && division.IsMine && (division.Goal == DivisionGoal.Defend))
                {
                    position = references.NormalizeWorldSpacePosition(division.transform.position) * minimapRect.size + minimapRect.position;
                    GUI.DrawTexture(new Rect(position.x - iconSize / 2, position.y - iconSize / 2, iconSize, iconSize), combatIcon);
                }
                else if (division != null && division.IsMine && (division.Goal != DivisionGoal.Defend))
                {
                    position = references.NormalizeWorldSpacePosition(division.transform.position) * minimapRect.size + minimapRect.position;
                    GUI.DrawTexture(new Rect(position.x - iconSize / 2, position.y - iconSize / 2, iconSize, iconSize), icon);
                }
                
            }
        }
    }
}
