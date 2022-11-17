using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayActionsAbility : DisplayAbility
{
    public event EventHandler<ActionButtonCreatedEventArgs> ActionButtonCreated;

    protected override void Awake()
    {
        base.Awake();

    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        yield return 0;
    }

    public override void Display(TileGrid tileGrid)
    {
        foreach (Ability ability in GetComponents<Ability>())
        {
            ability.UnitReference = UnitReference;
            if (ability.IsDisplayable && ability.CanPerform(tileGrid))
            {
                ActionButtonCreated?.Invoke(this, new ActionButtonCreatedEventArgs(ActWrapper(ability, tileGrid), ability.Name));
            }
        }
    }

    public IEnumerator ActWrapper(Ability ability, TileGrid tileGrid)
    {
        return (Execute(tileGrid,
                _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, ability)));
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

public class ActionButtonCreatedEventArgs : EventArgs
{
    public IEnumerator ButtonAction;
    public string ButtonName;

    public ActionButtonCreatedEventArgs(IEnumerator buttonAction, string buttonName)
    {
        ButtonAction = buttonAction;
        ButtonName = buttonName;
    }
}

