using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TileGridStateUnitSelected : TileGridState
{
    Ability _ability;
    Unit _unit;

    public TileGridStateUnitSelected(TileGrid tileGrid, Unit unit, Ability ability) : base(tileGrid)
    {
        _ability = ability;
        _unit = unit;
    }

    public override void OnUnitClicked(Unit unit)
    {
        _ability.OnUnitClicked(unit, _tileGrid);
    }
    public override void OnUnitHighlighted(Unit unit)
    {
        _ability.OnUnitHighlighted(unit, _tileGrid);
    }
    public override void OnUnitDehighlighted(Unit unit)
    {
        _ability.OnUnitDehighlighted(unit, _tileGrid);
    }
    public override void OnTileClicked(OverlayTile tile)
    {
        _ability.OnTileClicked(tile, _tileGrid);
    }
    public override void OnTileSelected(OverlayTile tile)
    {
        _ability.OnTileSelected(tile, _tileGrid);
    }
    public override void OnTileDeselected(OverlayTile tile)
    {
        _ability.OnTileDeselected(tile, _tileGrid);
    }

    public override void OnRightClick()
    {
        _ability.OnRightClick(_tileGrid);
    }

    public override void OnStateEnter()
    {   
        if(_ability is MoveAbility)
            _unit?.OnUnitSelected();

        _ability.OnAbilitySelected(_tileGrid);
        _ability.Display(_tileGrid);          

    }
    public override void OnStateExit()
    {
        if(_ability is ResetAbility or HealAbility or AttackAbility or WaitAbility)
            _unit?.OnUnitDeselected();

        _ability.OnAbilityDeselected(_tileGrid);
        _ability.CleanUp(_tileGrid);

    }
}


