using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayWeaponsAbility : DisplayAbility
{
    public GameObject ActionButton;

    public override IEnumerator Act(TileGrid tileGrid)
    {
        yield return 0;
    }

    public override void Display(TileGrid tileGrid)
    {
        foreach (Weapon w in UnitReference.AvailableWeapons)
        {
            OnButtonCreated(ActWrapper(w, tileGrid), w.Name);
        }
    }

    public IEnumerator ActWrapper(Weapon w, TileGrid tileGrid)
    {
        return Execute(tileGrid,
                _ => GetComponent<EquipAbility>().WeaponToEquip = w,
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, GetComponent<EquipAbility>()));
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.HideCursor();
    }

    public override void OnTileSelected(OverlayTile tile, TileGrid tileGrid)
    {
        tile.UnMark();
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        tile.UnMark();
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return true;
    }
}

