using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DisplayHealStatsAbility : DisplayAbility
{
    public event EventHandler<DisplayHealStatsChangedEventArgs> DisplayHealStatsChanged;

    private HashSet<OverlayTile> _tilesInHealRange;

    protected override void Awake()
    {
        base.Awake();
        Name = "Heal";
        IsDisplayableAsButton = false;
    }
    public override void Display(TileGrid tileGrid)
    {
        foreach (var t in _tilesInHealRange)
            t.MarkAsHealableTile();
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        if (UnitReference.IsUnitHealable(unit) && !UnitReference.Equals(unit))
        {
            var direction = UnitReference.GetDirectionToFace(unit.Tile.transform.position);
            UnitReference.SetState(new UnitStateMoving(UnitReference, direction));
            unit.Tile.HighlightedOnUnit();
            var healStats = GetStats(UnitReference, unit);
            DisplayHealStatsChanged?.Invoke(this, new DisplayHealStatsChangedEventArgs(healStats));

        }
    }

    public override void OnUnitDehighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.DeHighlightedOnUnit();
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        if (tile.CurrentUnit)
            OnUnitClicked(tile.CurrentUnit, tileGrid);
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        if (UnitReference.IsUnitHealable(unit) && !UnitReference.Equals(unit))
        {
            UnitReference.GetComponentInChildren<HealAbility>().UnitToHeal = unit;
            StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<HealAbility>()));
        }
    }

    public override void OnRightClick(TileGrid tileGrid)
    {
        StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<SelectStaffToHealAbility>()));
    }


    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);

        _tilesInHealRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.EquippedStaff.Range);

    }

    protected virtual HealStats GetStats(Unit healer, Unit ally)
    {
        return new HealStats(healer, ally);
    }
}

public class DisplayHealStatsChangedEventArgs : EventArgs
{
    public HealStats healStat;

    public DisplayHealStatsChangedEventArgs(HealStats stat)
    {
        healStat = stat;
    }
}

