using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DisplayHealStatsAbility), typeof(DisplayActionsAbility))]
public class SelectStaffToHealAbility : SelectItemToUseAbility
{
    protected override void Awake()
    {
        base.Awake();
        Name = "Staff";
        IsDisplayable = true;
    }

    public override void Display(TileGrid tileGrid)
    {
        foreach (Staff s in UnitReference.AvailableStaffs)
        {
            OnButtonCreated(ActWrapper(s, tileGrid), s.Name, s);
        }
    }

    public IEnumerator ActWrapper(Staff s, TileGrid tileGrid)
    {
        return Execute(tileGrid,
                _ => UnitReference.EquipItem(s),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, GetComponent<DisplayHealStatsAbility>()));
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        //if user clicks outside the selection boxm return back to display actions
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<DisplayActionsAbility>())));
        }
    }


    //Can Select Weapon to Attack if the Unit has a weapon
    public override bool CanPerform(TileGrid tileGrid)
    {
        if (UnitReference.AvailableStaffs.Count <= 0)
        {
            return false;
        }

        var allyUnits = tileGrid.GetPlayerUnits(tileGrid.CurrentPlayer);
        var alliesInRange = allyUnits.Where(u => UnitReference.IsUnitHealable(u)).ToList();

        return alliesInRange.Count > 0 && UnitReference.ActionPoints > 0;
    }
}

