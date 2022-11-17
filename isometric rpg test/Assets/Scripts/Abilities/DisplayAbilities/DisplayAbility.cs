using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//change to displayStats Ability
public class DisplayAbility : Ability
{

    protected List<OverlayTile> tilesInAttackRange;

    public override void CleanUp(TileGrid tileGrid)
    {
        tilesInAttackRange.ForEach(t => t.UnMark());
    }


    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);
        tilesInAttackRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.EquippedWeapon.Range).Where(t => t != UnitReference.Tile).ToList();
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        base.OnAbilityDeselected(tileGrid);
    }
}
