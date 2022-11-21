using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DisplayAttackStatsAbility : DisplayAbility
{
    public event EventHandler<DisplayStatsChangedEventArgs> DisplayStatsChanged;

    private List<OverlayTile> _tilesInAttackRange;

    protected override void Awake()
    {
        base.Awake();
    }
    public override void Display(TileGrid tileGrid)
    {                           
        _tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());
        UnitReference.Tile.UnMark();
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.HighlightedOnUnit();

        if (UnitReference.IsUnitAttackable(unit, true) && !UnitReference.Equals(unit))
        {
            //face the unit we're highlighting
            UnitReference.SetMove(UnitReference.GetDirectionToFace(unit.transform.position));

            var range = tileGrid.GetManhattenDistance(UnitReference.Tile, unit.Tile);
            var attackerStats = GetStats(UnitReference, unit, range);
            var defenderStats = GetStats(unit, UnitReference, range);

            if (DisplayStatsChanged != null)
                DisplayStatsChanged.Invoke(this, new DisplayStatsChangedEventArgs(attackerStats, defenderStats));
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<SelectWeaponToAttackAbility>())));     
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

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);

        _tilesInAttackRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.EquippedWeapon.Range);
    }

    protected CombatStats GetStats(Unit unit, Unit unitToAttack, int range)
    {
        return new CombatStats(unit, unitToAttack, range);
    }
}


public class DisplayStatsChangedEventArgs : EventArgs
{
    public CombatStats AttackerStats;
    public CombatStats DefenderStats;

    public DisplayStatsChangedEventArgs(CombatStats attackerStats, CombatStats defenderStats)
    {
        AttackerStats = attackerStats;
        DefenderStats = defenderStats;
    }
}
