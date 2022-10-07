using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public class RangeFinder
{

    //FloodFill
    public List<OverlayTile> GetTilesInRange(Unit unit, List<OverlayTile> tiles, int range)
    {
        AssignTileRating(unit, range);

        var tilesInRange = new List<OverlayTile>();

        tilesInRange.Add(unit.Tile);

        int count = 0;

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            //Get each tile's distinct neighbor
            tilesInRange.ForEach(t => surroundingTiles.AddRange(t.GetNeighborTiles(new List<OverlayTile>()).Where(n => !n.IsBlocked).Distinct().ToList()));

            foreach(var tile in surroundingTiles.ToList())
            {
                tile.MovementRating = tile.MovementRating - count - tile.MovementCost;
                
                if(tile.MovementRating < 1)
                {
                    surroundingTiles.Remove(tile);
                }
            }
         
            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRange;
    }

    private void AssignTileRating(Unit unit, int range)
    {
        var tilesInRange = new List<OverlayTile>();

        tilesInRange.Add(unit.Tile);

        int count = 0;

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            //Add each tile's neighbor to surrounding tiles
            tilesInRange.ForEach(t => surroundingTiles.AddRange(t.GetNeighborTiles(new List<OverlayTile>())));

            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }
        tilesInRange.ForEach(t => t.MovementRating = unit.MovementPoints);
    }

    
    public List<OverlayTile> GetTilesInAttackRange(Unit unit, List<OverlayTile> tiles, int range)
    {
        //create tilesInRange, which is a list of tiles in range of the starting tile
        var tilesInRange = new List<OverlayTile>();

        int count = 0;

        tilesInRange.AddRange(tiles);

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            tilesInRange.ForEach(t => surroundingTiles.AddRange(t.GetNeighborTiles(new List<OverlayTile>())));

            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRange;
    }

}

   