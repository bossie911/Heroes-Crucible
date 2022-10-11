using GameStudio.HunterGatherer.Divisions;
using GameStudio.HunterGatherer.Divisions.UI;
using GameStudio.HunterGatherer.Networking;
using GameStudio.HunterGatherer.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisionOverviewControlGroups : MonoBehaviour
{
    private List<KeyCode> keysToCheck = new List<KeyCode>();

    //Array size is max amount of divisions you want a player to have.
    private Division[] currentPlayerDivisions = new Division[6];

    // Start is called before the first frame update
    void Start()
    {
        AddKeysToCheck();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            CheckForValidKeyPress();
        }

        ResetHotkeyDivisions(DivisionOverview.Instance.divisionItems);
    }

    //Function where you add all keys you want the script to check inputs for
    private void AddKeysToCheck()
    {
        keysToCheck.Add(KeyCode.Alpha1);
        keysToCheck.Add(KeyCode.Alpha2);
        keysToCheck.Add(KeyCode.Alpha3);
        keysToCheck.Add(KeyCode.Alpha4);
        keysToCheck.Add(KeyCode.Alpha5);
        keysToCheck.Add(KeyCode.Alpha6);
    }

    //Clears the array and aligns the hotkey with the division (In update for now, call when troops are added/removed later)
    private void ResetHotkeyDivisions(Dictionary<Division, DivisionOverviewItem> activeDivisions)
    {
        Array.Clear(currentPlayerDivisions, 0, currentPlayerDivisions.Length);

        foreach (KeyValuePair<Division, DivisionOverviewItem> dictItem in activeDivisions)
        {
            AddToArray(dictItem.Key);
        }
    }

    //Adds an item to the first empty slot in an array
    private void AddToArray(Division division)
    {
        int firstEmpty = System.Array.IndexOf(currentPlayerDivisions, null);
        currentPlayerDivisions[firstEmpty] = division;
    }

    //Checks whether pressed key is in the list of keys you want to check
    private void CheckForValidKeyPress()
    {
        for (int i = 0; i < keysToCheck.Count; i++)
        {
            if(Input.GetKeyDown(keysToCheck[i]))
            {
                SelectGroup(currentPlayerDivisions[i]);
            }
        }
    }

    //Adds the division in a list of selectableobjects that the SelectionManager can use to select your division
    private void SelectGroup(Division divisionToSelect)
    {
        if(divisionToSelect != null)
        {
            List<SelectableObject> selection = new List<SelectableObject>();
            selection.Add(divisionToSelect.GetComponent<SelectableObject>());
            SelectionManager.Instance.SelectObjects(selection);
        }
    }
}
