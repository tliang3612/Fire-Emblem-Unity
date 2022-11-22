using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SelectItemToUseAbility : DisplayAbility
{
    protected override void Awake()
    {
        base.Awake();
        IsDisplayable = true;
    }

    public event EventHandler<EquipButtonCreatedEventArgs> EquipButtonCreated;

    public override IEnumerator Act(TileGrid tileGrid)
    {
        yield return 0;
    }

    protected virtual void OnButtonCreated(IEnumerator action, string name, Item i)
    {
        EquipButtonCreated?.Invoke(this, new EquipButtonCreatedEventArgs(action, name, i, UnitReference));
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

    public override void OnRightClick(TileGrid tileGrid)
    {
        StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<DisplayActionsAbility>()));
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
    public Item Item;
    public Unit Unit;

    public EquipButtonCreatedEventArgs(IEnumerator buttonAction, string buttonName, Item i, Unit unitReference)
    {
        ButtonAction = buttonAction;
        ButtonName = buttonName;
        Item = i;
        Unit = unitReference;
    }
}

