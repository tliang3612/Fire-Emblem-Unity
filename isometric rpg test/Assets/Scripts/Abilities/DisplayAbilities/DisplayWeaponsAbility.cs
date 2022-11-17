using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayWeaponsAbility : DisplayAbility
{
    protected override void Awake()
    {
        base.Awake();
        Name = "Equip";
        IsDisplayable = true;
    }

    public event EventHandler<EquipButtonCreatedEventArgs> EquipButtonCreated;

    public override IEnumerator Act(TileGrid tileGrid)
    {
        yield return 0;
    }

    protected virtual void OnButtonCreated(IEnumerator action, string name, Weapon w)
    {
        EquipButtonCreated?.Invoke(this, new EquipButtonCreatedEventArgs(action, name, w, UnitReference));
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

public class EquipButtonCreatedEventArgs : EventArgs
{
    public IEnumerator ButtonAction;    
    public string ButtonName;
    public Weapon Weapon;
    public Unit unit;

    public EquipButtonCreatedEventArgs(IEnumerator buttonAction, string buttonName, Weapon w, Unit unitReference)
    {
        ButtonAction = buttonAction;
        ButtonName = buttonName;
        Weapon = w;
        unit = unitReference;
    }
}

