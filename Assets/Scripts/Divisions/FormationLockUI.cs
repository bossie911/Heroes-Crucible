using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class FormationLockUI : MonoBehaviour
{
    [SerializeField]
    private Toggle toggle;

    [SerializeField]
    private FormationLock formationLock;
    
    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(OnFormationLockToggle);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnFormationLockToggle);
    }

    private void OnFormationLockToggle(bool isOn)
    {
        formationLock.ActivateFormationLock(isOn);
    }
}
