using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayActionsAbility : Ability
{
    public event EventHandler<ButtonCreatedEventArgs> ButtonCreated;

    public GameObject ActionButton;

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
                if (ButtonCreated != null)
                    ButtonCreated.Invoke(this, new ButtonCreatedEventArgs(ActWrapper(ability, tileGrid), ability.Name));
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

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        base.OnAbilityDeselected(tileGrid);
    }

    public override void CleanUp(TileGrid tileGrid)
    {
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return true;
    }
}

public class ButtonCreatedEventArgs : EventArgs
{
    public IEnumerator ButtonAction;
    public string ButtonName;

    public ButtonCreatedEventArgs(IEnumerator buttonAction, string buttonName)
    {
        ButtonAction = buttonAction;
        ButtonName = buttonName;
    }
}
