using System.Linq;


public class TileGridStateWaitingForInput : TileGridState
{

    public TileGridStateWaitingForInput(TileGrid tileGrid) : base(tileGrid)
    {

    }

    public override void OnUnitClicked(Unit unit)
    {
        if (_tileGrid.GetCurrentPlayerUnits().Contains(unit))
        {
            _tileGrid.GridState = new TileGridStateUnitSelected(_tileGrid, unit, unit.GetComponentInChildren<MoveAbility>());
        }
    }

    public override void OnUnitHighlighted(Unit unit)
    {
        unit.Tile.HighlightedOnUnit();

        if (_tileGrid.GetCurrentPlayerUnits().Contains(unit))
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
}


