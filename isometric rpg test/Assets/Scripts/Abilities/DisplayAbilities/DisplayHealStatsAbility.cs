using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DisplayHealStatsAbility : DisplayAbility
{
    public event EventHandler<DisplayHealStatsChangedEventArgs> DisplayHealStatsChanged;

    private List<OverlayTile> _tilesInHealRange;

    protected override void Awake()
    {
        base.Awake();
        Name = "Heal";
        IsDisplayable = false;
    }
    public override void Display(TileGrid tileGrid)
    {
        _tilesInHealRange.ForEach(t => t.MarkAsHealableTile());
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.HighlightedOnUnit();

        if (UnitReference.IsUnitHealable(unit) && !UnitReference.Equals(unit))
        {
            var healStats = GetStats(UnitReference, unit);
            DisplayHealStatsChanged?.Invoke(this, new DisplayHealStatsChangedEventArgs(healStats));

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
        if (UnitReference.IsUnitHealable(unit) && !UnitReference.Equals(unit))
        {
            UnitReference.GetComponentInChildren<HealAbility>().UnitToHeal = unit;
            StartCoroutine(Execute(tileGrid,
                _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<HealAbility>())));
        }
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

