public class TileGridStateBlockInput : TileGridState
{

    public TileGridStateBlockInput(TileGrid tileGrid) : base(tileGrid)
    {
        
    }

    public override void OnTileSelected(OverlayTile tile)
    {
        tile.UnMark();
    }

    public override void OnTileDeselected(OverlayTile tile)
    {
        tile.UnMark();
    }

    public override void OnUnitHighlighted(Unit unit)
    {

    }


}


