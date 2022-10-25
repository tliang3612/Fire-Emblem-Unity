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
    /// Method is called when a unit is clicked on.
    /// </summary>
    public virtual void OnUnitClicked(Unit unit)
    {
    }

    public virtual void OnUnitHighlighted(Unit unit)
    {
        unit.Tile.HighlightedOnUnit();
    }

    public virtual void OnUnitDehighlighted(Unit unit)
    {
        unit.Tile.DeHighlightedOnUnit();
    }


    /// <summary>
    /// Method is called when mouse exits cell's collider.
    /// </summary>
    /// <param name="cell">Cell that was deselected.</param>
    public virtual void OnTileDeselected(OverlayTile tile)
    {
        tile.MarkAsDeHighlighted();
    }

    /// <summary>
    /// Method is called when mouse enters cell's collider.
    /// </summary>
    public virtual void OnTileSelected(OverlayTile tile)
    {
        tile.MarkAsHighlighted();
    }

    /// <summary>
    /// Method is called when a cell is clicked.
    /// </summary>
    /// <param name="cell">Cell that was clicked.</param>
    public virtual void OnTileClicked(OverlayTile tile)
    {
    }

    /// <summary>
    /// Method is called on transitioning into a state.
    /// </summary>
    public virtual void OnStateEnter()
    {
        foreach (var tile in _tileGrid.TileList)
        {
            tile.UnMark();
        }
    }

    /// <summary>
    /// Method is called on transitioning out of a state.
    /// </summary>
    public virtual void OnStateExit()
    {
        foreach (var tile in _tileGrid.TileList)
        {
            tile.UnMark();
        }
    }
}