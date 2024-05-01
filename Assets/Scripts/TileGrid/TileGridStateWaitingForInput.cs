using System.Linq;


public class TileGridStateWaitingForInput : TileGridState
{
    public TileGridStateWaitingForInput(TileGrid tileGrid) : base(tileGrid)
    {

    }

    public override void OnUnitClicked(Unit unit)
    {
        _tileGrid.TileList.ForEach(t => t.UnMark());
        if (_tileGrid.GetCurrentPlayerUnits().Contains(unit))
        {
             if(unit.ActionPoints > 0)
                _tileGrid.GridState = new TileGridStateUnitSelected(_tileGrid, unit, unit.GetComponentInChildren<MoveAbility>());
        }
        else
        {
            unit.GetTilesInAttackRange(_tileGrid).ToList().ForEach(t => t.MarkAsAttackableTile());
        }
    }

    public override void OnUnitHighlighted(Unit unit)
    {
        unit.Tile.HighlightedOnUnit();

        if (_tileGrid.CurrentPlayer is HumanPlayer && _tileGrid.GetCurrentPlayerUnits().Contains(unit) && unit.ActionPoints > 0)
        {
            unit.SetState(new UnitStateHovered(unit));
        }
    }

    public override void OnUnitDehighlighted(Unit unit)
    {
        unit.Tile.DeHighlightedOnUnit();

        if (_tileGrid.GetCurrentPlayerUnits().Contains(unit))
        {
            unit.SetState(new UnitStateNormal(unit));
        }
    }

    public override void OnTileSelected(OverlayTile tile)
    {
        tile.MarkAsHighlighted();
    }

    public override void OnTileDeselected(OverlayTile tile)
    {
        tile.MarkAsDeHighlighted();
    }

    public override void OnRightClick()
    {
        _tileGrid.TileList.ForEach(t => t.UnMark());
    }
}


