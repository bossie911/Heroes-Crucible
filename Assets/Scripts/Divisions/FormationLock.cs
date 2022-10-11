using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStudio.HunterGatherer.Selection;
using GameStudio.HunterGatherer.Divisions;
using UnityEngine.UI;

public class FormationLock : MonoBehaviour {
    public bool formationLocked = false;
    public static FormationLock Instance {
        get; private set;
    }
    public Dictionary<Division, DivisionLockInfo> divisionLockInfo;
    public Vector3 centerPosition = Vector3.zero;

    public struct DivisionLockInfo {
        public Vector3 offset;
        //public float rotation;

        public DivisionLockInfo(Vector3 offset) {
            this.offset = offset;
            //this.rotation = rotation;
        }
    }

    private void Awake() {
        SelectionManager.Instance.onDivisionSelected.AddListener(OnDivisionSelected);
        divisionLockInfo = new Dictionary<Division, DivisionLockInfo>();

        if(Instance == null) {
            Instance = this;
        } else if(Instance != this) {
            GameObject.Destroy(gameObject);
            return;
        }
    }

    public static float RotationOffset;

    public void LockFormation() {
        List<Division> selectedDivisions = SelectionManager.Instance.SelectedOwnedDivisions;
        centerPosition = CalculateAvaragePosition(selectedDivisions);

        // Calculate total rotation offset
        RotationOffset = 0;
        foreach(Division division in selectedDivisions)
        {
            float angle = division.FirstUnit.transform.eulerAngles.y;
            if (angle < 0)
                angle += 360;
            RotationOffset += angle;
        }
        RotationOffset /= selectedDivisions.Count;

        // Calculate offsets per division
        foreach(Division division in selectedDivisions) {
            Vector3 offset = CalculateCenterDivision(division) - centerPosition;
            divisionLockInfo.Add(division, new DivisionLockInfo(offset));
        }
    }

    public void ClearDictionary() {
        divisionLockInfo.Clear();
        centerPosition = Vector3.zero;
    }

    public void OnDivisionSelected() {
        ClearDictionary();
        LockFormation();
    }

    public Vector3 CalculateAvaragePosition(List<Division> selectableObjects) {
        Vector3 averageDivisionPosition = Vector3.zero;

        selectableObjects.ForEach(division => {
            averageDivisionPosition += CalculateCenterDivision(division);
        });

        averageDivisionPosition /= SelectionManager.Instance.SelectedObjects.Count;
        return averageDivisionPosition;
    }

    private Vector3 CalculateCenterDivision(Division division) {
        Vector3 centerDivisionPosition = Vector3.zero;

        foreach (var unit in division.Units)
        {
            centerDivisionPosition += unit.transform.position;
        }

        centerDivisionPosition /= division.Units.Count;

        return centerDivisionPosition;
    }

    public void ActivateFormationLock(bool formationIsLocked) {
        if(formationIsLocked) {
            Lock();
        } else {
            Unlock();
        }
    }

    public void Lock() {
        formationLocked = true;
        ClearDictionary();
        LockFormation();
    }

    public void Unlock() {
        formationLocked = false;
        ClearDictionary();
    }
}