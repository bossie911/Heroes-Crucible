using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.Divisions.UI
{
    /// <summary> Handles the healthbar of a division in worldspace </summary>
    public class WorldSpaceDivisionHealth : MonoBehaviour
    {
        [SerializeField] private Color allyColor = Color.blue;

        [SerializeField]
        private HealthBar bar = null;

        [SerializeField]
        private Canvas canvas = null;

        [SerializeField]
        private Division division = null;
        [SerializeField]
        private GameObject heroIcon = null;
        [SerializeField]
        private GameObject pikeIcon = null;
        [SerializeField]
        private GameObject swordIcon = null;
        [SerializeField]
        private GameObject bowIcon = null;
        [SerializeField]
        private GameObject nomadIcon = null;
        [SerializeField]
        private Image flag = null;
        [SerializeField]
        private Color[] colors;


        private void Start()
        {
            //Set the canvas camera to the UI camera so that healthbars are rendered above the outline shader.
            Camera[] allCams = new Camera[Camera.allCamerasCount];
            Camera.GetAllCameras(allCams);

            foreach (Camera cam in allCams)
            {
                if (cam.gameObject.name == "UI Camera")
                {
                    canvas.worldCamera = cam;
                    break;
                }
            }



            division.OnDivisionTypeChanged.AddListener(SetDivisionTypeSpecificUI);
            bar.Initialize(division);
            SetDivisionTypeSpecificUI();
        }

        private void Update()
        {
            CentreHealthbarPosition();
        }

        /// <summary> Event listener that disables the healthbar when the division dies </summary>
        private void Division_OnStateChanged(UnitState unitState, bool active)
        {
            if (active && unitState == UnitState.Death && division.Units.Count == 0) //Last man standing
            {
                // Remove event listeners and turn healthbar off
                foreach (Unit unit in division.Units)
                {
                    unit.OnStateChanged.RemoveListener(Division_OnStateChanged);
                }

                this.gameObject.SetActive(false);
            }
        }

        /// <summary> Set the health bar position in the cnetre of the units in the division. </summary>
        private void CentreHealthbarPosition()
        {
            Vector3 combinedPositions = Vector3.zero;
            for (int i = 0; i < division.Units.Count; i++)
            {
                if (division.Units[i] != null)
                {
                    combinedPositions += division.Units[i].transform.position;
                }
            }

            Vector3 newPos = combinedPositions / division.Units.Count;
            if (float.IsNaN(newPos.x) || float.IsNaN(newPos.y) || float.IsNaN(newPos.z))
            {
                return;
            }

            division.transform.position = newPos;
        }

        /// <summary>
        /// checks if the division is visible
        /// </summary>
        private void CheckDivisionVisibility()
        {
            canvas.enabled = division.IsDivisionVisible();
        }

        /// <summary> Set the health bar style and the icon depending on the type of the division </summary>
        private void SetDivisionTypeSpecificUI()
        {
            nomadIcon.SetActive(false);
            heroIcon.SetActive(false);
            pikeIcon.SetActive(false);
            swordIcon.SetActive(false);
            bowIcon.SetActive(false);

            if (division.TypeData == null)
                return;

            switch (division.TypeData.Type)
            {
                case DivisionType.Hero:
                    if (heroIcon)
                    {
                        heroIcon.SetActive(true);
                    }

                    break;
                case DivisionType.Pikemen:
                    if (pikeIcon)
                    {
                        pikeIcon.SetActive(true);
                    }

                    break;
                case DivisionType.Swordsmen:
                    if (swordIcon)
                    {
                        swordIcon.SetActive(true);
                    }

                    break;
                case DivisionType.Nomad:
                    if (nomadIcon)
                    {
                        nomadIcon.SetActive(true);
                    }

                    break;
                case DivisionType.Archers:
                    if (bowIcon)
                    {
                        bowIcon.SetActive(true);
                    }

                    break;
            }
        }

        private void OnEnable()
        {
            //add listener for when the health-bar is assigned to a new player.
            division.OnDivisionTypeChanged.AddListener(SetDivisionTypeSpecificUI);
            //only runs after the first time the health bar is created.
            if (division)
            {
                SetDivisionTypeSpecificUI();
            }
        }

        private void FixedUpdate()
        {
            CheckDivisionVisibility();
        }
    }
}