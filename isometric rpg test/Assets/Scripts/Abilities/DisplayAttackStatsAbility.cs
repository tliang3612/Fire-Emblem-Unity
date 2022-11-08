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

        if (UnitReference.IsUnitAttackable(unit) && !UnitReference.Equals(unit))
        {
            var attackerStats = GetStats(UnitReference, unit);
            var defenderStats = GetStats(unit, UnitReference);

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
        if (UnitReference.IsUnitAttackable(unit) && !UnitReference.Equals(unit))
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
        var enemiesInRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u)).ToList();

        return enemiesInRange.Count > 0 && UnitReference.ActionPoints > 0;
    }

    protected DisplayStats GetStats(Unit unit, Unit unitToAttack)
    {
        return new DisplayStats(null, unit.UnitName, unit.HitPoints, unit.GetTotalDamage(unitToAttack),
            unit.GetBattleAccuracy(unitToAttack), unit.GetCritChance(), unit.GetEffectiveness(unitToAttack));
    }
}
