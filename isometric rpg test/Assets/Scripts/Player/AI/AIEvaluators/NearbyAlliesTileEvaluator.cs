using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyAlliesTileEvaluator : TileEvaluator
{
    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid)
    {

    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        var neighbours = tileToEvaluate.GetNeighborTiles(tileGrid);
        var nearbyAllies = 0;

        foreach(var neighbor in neighbours)
        {
            if (neighbor.CurrentUnit && neighbor.CurrentUnit.PlayerNumber == evaluatingUnit.PlayerNumber)
                nearbyAllies += 1;
        }
        return (float)nearbyAllies / neighbours.Count;
    }    
}
