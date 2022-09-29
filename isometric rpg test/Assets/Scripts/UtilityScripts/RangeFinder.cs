using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RangeFinder
{

    public List<OverlayTile> GetTilesInRange(TileGrid tileGrid, OverlayTile startingTile, int range)
    {
        //create tilesInRange, which is a list of tiles in range of the starting tile
        var tilesInRange = new List<OverlayTile>();

        int count = 0;

        tilesInRange.Add(startingTile);

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            tilesInRange.ForEach(t => surroundingTiles.AddRange(tileGrid.GetNeighborTiles(t, new List<OverlayTile>())));

            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRange;
    }

    public List<OverlayTile> GetTilesInSetRange(TileGrid tileGrid, OverlayTile startingTile, int rangeMin, int rangeMax)
    {
        var tilesInRangeToExclude = new List<OverlayTile>();
        var tilesInRangeToInclude = new List<OverlayTile>();

        int count = 0;

        tilesInRangeToExclude.Add(startingTile);
        while (count < rangeMin)
        {
            var surroundingTiles = new List<OverlayTile>();

            tilesInRangeToExclude.ForEach(t => surroundingTiles.AddRange(tileGrid.GetNeighborTiles(t, new List<OverlayTile>())));

            //adds the surrounding distinct tiles to tilesInRange
            tilesInRangeToExclude.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        tilesInRangeToInclude.AddRange(tilesInRangeToExclude);

        while (count < rangeMax)
        {
            var surroundingTiles = new List<OverlayTile>();
            tilesInRangeToInclude.ForEach(t => surroundingTiles.AddRange(tileGrid.GetNeighborTiles(t, new List<OverlayTile>())));

            tilesInRangeToInclude.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRangeToInclude.Except(tilesInRangeToExclude).ToList();
    }
}
