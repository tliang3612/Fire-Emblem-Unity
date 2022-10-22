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
            _tileGrid.GridState = new TileGridStateUnitSelected(_tileGrid, unit, unit.GetComponents<Ability>().ToList());
        }
    }
}


