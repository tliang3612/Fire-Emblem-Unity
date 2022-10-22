using System.Collections.Generic;
using Units.UnitStates;
using UnityEngine;
using System.Linq;
public class TileGridStateUnitSelected : TileGridState
{
    List<Ability> _abilities;
    Unit _unit;

    public TileGridStateUnitSelected(TileGrid tileGrid, Unit unit, List<Ability> abilities) : base(tileGrid)
    {
        if (abilities.Count == 0)
        {
            Debug.LogError("No abilities were selected, check if your unit has any abilities attached to it");
        }

        _abilities = abilities;
        _unit = unit;
    }

    public TileGridStateUnitSelected(TileGrid tileGrid, Unit unit, Ability ability) : this(tileGrid, unit, new List<Ability>() { ability }) { }

    public override void OnUnitClicked(Unit unit)
    {
        _abilities.ForEach(a => a.OnUnitClicked(unit, _tileGrid));
    }
    public override void OnUnitHighlighted(Unit unit)
    {
        _abilities.ForEach(a => a.OnUnitHighlighted(unit, _tileGrid));
    }
    public override void OnUnitDehighlighted(Unit unit)
    {
        _abilities.ForEach(a => a.OnUnitDehighlighted(unit, _tileGrid));
    }
    public override void OnTileClicked(OverlayTile tile)
    {
        _abilities.ForEach(a => a.OnTileClicked(tile, _tileGrid));
    }
    public override void OnTileSelected(OverlayTile tile)
    {
        base.OnTileSelected(tile);
        _abilities.ForEach(a => a.OnTileSelected(tile, _tileGrid));
    }
    public override void OnTileDeselected(OverlayTile tile)
    {
        base.OnTileDeselected(tile);
        _abilities.ForEach(a => a.OnTileDeselected(tile, _tileGrid));
    }

    public override void OnStateEnter()
    {
        _unit?.OnUnitSelected();
        _abilities.ForEach(a => a.OnAbilitySelected(_tileGrid));
        _abilities.ForEach(a => a.Display(_tileGrid));
            

        var canPerformAction = _abilities.Select(a => a.CanPerform(_tileGrid))
                                            .DefaultIfEmpty()
                                            .Aggregate((result, next) => result || next);

        if (!canPerformAction)
        {       
            _unit?.SetState(new UnitStateFinished(_unit));
        }

    }
    public override void OnStateExit()
    {
        _unit?.OnUnitDeselected();
        _abilities.ForEach(a => a.OnAbilityDeselected(_tileGrid));
        _abilities.ForEach(a => a.CleanUp(_tileGrid));

    }
}


