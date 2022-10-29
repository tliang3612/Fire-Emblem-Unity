using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayAttackStatsAbility : Ability
{
    public event EventHandler<DisplayStatsChangedEventArgs> DisplayStatsChanged;

    private List<OverlayTile> tilesInAttackRange;

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

    public override void CleanUp(TileGrid tileGrid)
    {
        tilesInAttackRange.ForEach(t => t.UnMark());
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        if (UnitReference.IsUnitAttackable(unit))
        {
            unit.Tile.HighlightedOnUnit();
            //Change StatsDisplay stats
            var attackerStats = GetStats(UnitReference, unit);
            var defenderStats = GetStats(unit, UnitReference);

            if (DisplayStatsChanged != null)
                DisplayStatsChanged.Invoke(this, new DisplayStatsChangedEventArgs(attackerStats, defenderStats));
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
        if (UnitReference.IsUnitAttackable(unit))
        {
            UnitReference.GetComponentInChildren<AttackAbility>().UnitToAttack = unit;
            UnitReference.GetComponentInChildren<AttackAbility>().OnAbilitySelected(tileGrid);
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);
        tilesInAttackRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.AttackRange).Where(t => t != UnitReference.Tile).ToList();
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        base.OnAbilityDeselected(tileGrid);
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        var enemiesInRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u)).ToList();

        return enemiesInRange.Count > 0 && UnitReference.ActionPoints > 0;
    }

    private DisplayStats GetStats(Unit unit, Unit unitToAttack)
    {
        return new DisplayStats(unit.HitPoints, unit.GetTotalDamage(unitToAttack),
            unit.GetBattleAccuracy(unitToAttack), unit.GetCritChance(), unit.UnitName);
    }

}

public class DisplayStatsChangedEventArgs : EventArgs
{
    public DisplayStats AttackerStats;
    public DisplayStats DefenderStats;

    public DisplayStatsChangedEventArgs(DisplayStats attackerStats, DisplayStats defenderStats)
    {
        AttackerStats = attackerStats;
        DefenderStats = defenderStats;
    }
}


public class DisplayStats
{
    public string Name { get; private set; }
    public int Hp { get; private set; }
    public int Damage { get; private set; }
    public int Hit { get; private set; }
    public int Crit { get; private set; }

    public DisplayStats(int health, int damage, int hitChance, int critChance, string unitName)
    {
        Name = unitName;
        Hp = health;
        Damage = damage;
        Hit = hitChance;
        Crit = critChance;
    }
}
