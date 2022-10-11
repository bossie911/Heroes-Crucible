using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStationIcon : MonoBehaviour
{
    [SerializeField] private Station myStation;
    [SerializeField]
    private Sprite archer, swordsmen, pikemen;

    private SpriteRenderer stationIcon;

    private Transform flagUsed;
    private Dictionary<UpgradeTypes, Sprite> stationSprites = new Dictionary<UpgradeTypes, Sprite>();

    private bool canUpdate;

    public void AssignCorrectFlag()
    {
        stationSprites.Add(UpgradeTypes.Archers, archer);
        stationSprites.Add(UpgradeTypes.Swordsmen, swordsmen);
        stationSprites.Add(UpgradeTypes.Pikemen, pikemen);
        
        stationIcon = transform.Find("StationIcon").GetComponent<SpriteRenderer>();
        
        stationIcon.sprite = stationSprites[myStation.stationType];

        flagUsed = transform.Find("FlagUsed");

        canUpdate = true;
    }
    
    private void FixedUpdate()
    {
        if (canUpdate)
        {
            RotateToCamera();
            ScaleWithCameraHeight(); 
        }
    }

    private void RotateToCamera()
    {
        flagUsed.LookAt(Camera.main.transform);
        stationIcon.transform.LookAt(Camera.main.transform);
    }

    private void ScaleWithCameraHeight()
    {
        stationIcon.transform.localScale = new Vector3(1,1,1) * Camera.main.transform.position.y * 0.03f;
        flagUsed.localScale = new Vector3(1, 1, 1) * Camera.main.transform.position.y * 0.0095f;

    }
}
    