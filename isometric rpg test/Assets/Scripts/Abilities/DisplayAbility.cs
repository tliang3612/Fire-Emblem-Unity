using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DisplayAbility : Ability
{
    public event EventHandler<DisplayStatsChangedEventArgs> DisplayStatsChanged;
    protected List<OverlayTile> tilesInAttackRange;

    public override void CleanUp(TileGrid tileGrid)
    {
        tilesInAttackRange.ForEach(t => t.UnMark());
    }

    //Invokes DisplayStatsChanged Event given the otherUnit
    protected virtual void OnDisplayStatsChanged(DisplayStats currentUnitStats, DisplayStats otherUnitStats)
    {   
        if (DisplayStatsChanged != null)
            DisplayStatsChanged.Invoke(this, new DisplayStatsChangedEventArgs(currentUnitStats, otherUnitStats));
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
    public Sprite Portrait { get; private set; }
    public string Name { get; private set; }
    public int Hp { get; private set; }
    public int Damage { get; private set; } //Damage can be damage dealt to a unit, or amount healed
    public int Hit { get; private set; }
    public int Crit { get; private set; }
    public int Effectiveness { get; private set; }

    public DisplayStats(Sprite portrait, string unitName, int health, int damage, int hitChance, int critChance, int effectiveness)
    {
        Portrait = portrait;
        Name = unitName;
        Hp = health;
        Damage = damage;
        Hit = hitChance;
        Crit = critChance;
        Effectiveness = effectiveness;        
    }
}
