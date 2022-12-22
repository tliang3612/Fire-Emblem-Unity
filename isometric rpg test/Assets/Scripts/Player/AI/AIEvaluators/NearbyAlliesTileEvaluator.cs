using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyAlliesTileEvaluator : TileEvaluator
{

    public override void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid)
    {

    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        var neighbours = tileToEvaluate.GetNeighborTiles(tileGrid);
        var nearbyAllies = 0;

        for (int i = 0; i < neighbours.Count; i++)
        {
            OverlayTile tile = neighbours[i];
            if (tile.CurrentUnit)
            {
                nearbyAllies += tile.CurrentUnit.PlayerNumber == evaluatingUnit.PlayerNumber && tile.CurrentUnit != evaluatingUnit ? 1 : 0;
            }
        }

        return nearbyAllies / neighbours.Count;
    }    
}
