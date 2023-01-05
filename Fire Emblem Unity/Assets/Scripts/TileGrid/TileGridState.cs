using UnityEngine;

public abstract class TileGridState
{
    protected TileGrid _tileGrid;

    protected TileGridState(TileGrid tileGrid)
    {
        _tileGrid = tileGrid;
    }

    public virtual TileGridState TransitionState(TileGridState nextState)
    {
        return nextState;
    }

    /// <summary>
    /// Method is called when a Unit is clicked on.
    /// </summary>
    public virtual void OnUnitClicked(Unit unit)
    {
    }

    public virtual void OnUnitHighlighted(Unit unit)
    {
    }

    public virtual void OnUnitDehighlighted(Unit unit)
    {
        unit.Tile.DeHighlightedOnUnit();
    }

    public virtual void OnTileDeselected(OverlayTile tile)
    {
    }

    public virtual void OnTileSelected(OverlayTile tile)
    {
        tile.MarkAsHighlighted();
    }

    public virtual void OnTileClicked(OverlayTile tile)
    {
    }

    public virtual void OnRightClick()
    {       
    }

    public virtual void OnStateEnter()
    {
        foreach (var tile in _tileGrid.TileList)
        {
            tile.UnMark();
        }
    }

    public virtual void OnStateExit()
    {
        foreach (var tile in _tileGrid.TileList)
        {
            tile.UnMark();
        }
    }
}