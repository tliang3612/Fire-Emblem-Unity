using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DisplayActionsAbility))]
public class SelectWeaponToEquipAbility : SelectItemToUseAbility
{
    protected override void Awake()
    {
        base.Awake();
        Name = "Equip";
        IsDisplayable = true;
    }

    public override void Display(TileGrid tileGrid)
    {
        foreach (Weapon w in UnitReference.AvailableWeapons)
        {
            OnButtonCreated(ActWrapper(w, tileGrid), w.Name, w);
        }
    }

    public IEnumerator ActWrapper(Weapon w, TileGrid tileGrid)
    {
        return Execute(tileGrid,
                _ => UnitReference.EquipItem(w),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, GetComponent<DisplayActionsAbility>()));
    }

    public override void OnRightClick(TileGrid tileGrid)
    {
        StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<DisplayActionsAbility>()));
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return true;
    }
}

