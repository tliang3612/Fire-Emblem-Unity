using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DisplayAttackStatsAbility : DisplayAbility
{
    protected override void Awake()
    {
        base.Awake();
        Name = "Attack";
        IsDisplayable = true;
    }
    public override void Display(TileGrid tileGrid)
    {                           
        tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());     
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.HighlightedOnUnit();

        if (UnitReference.IsUnitAttackable(unit, true) && !UnitReference.Equals(unit))
        {
            var range = tileGrid.GetManhattenDistance(UnitReference.Tile, unit.Tile);
            var attackerStats = GetStats(UnitReference, unit, range);
            var defenderStats = GetStats(unit, UnitReference, range);

            OnDisplayStatsChanged(attackerStats, defenderStats);
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<DisplayActionsAbility>())));     
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        if (UnitReference.IsUnitAttackable(unit, true) && !UnitReference.Equals(unit))
        {
            UnitReference.GetComponentInChildren<AttackAbility>().UnitToAttack = unit;
            StartCoroutine(Execute(tileGrid,
                _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<AttackAbility>())));
        }
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        var enemiesInRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, false)).ToList();

        return enemiesInRange.Count > 0 && UnitReference.ActionPoints > 0;
    }

    protected CombatStats GetStats(Unit unit, Unit unitToAttack, int range)
    {
        return new CombatStats(unit, unitToAttack, range);
    }
}
