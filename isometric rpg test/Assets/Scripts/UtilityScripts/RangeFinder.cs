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
        var tilesInRange = new List<OverlayTile>();
        unit.Tile.MovementCost = -1;
        unit.Tile.MovementRating = unit.MovementPoints - 1;

        tilesInRange.Add(unit.Tile);


        return Flood(unit.Tile, tilesInRange);
    }

    public List<OverlayTile> Flood(OverlayTile tile, List<OverlayTile> visitedTiles)
    {
        if (tile.MovementRating < 1)
            return visitedTiles;
       

        foreach (var n in tile.GetNeighborTiles(new List<OverlayTile>())) 
        {
            if (!n.IsBlocked)
            {
                if (!visitedTiles.Contains(n))
                {
                    visitedTiles.Add(n);
                    
                }
            }

            if(n.MovementRating > 0)
            {
                var originalRating = n.MovementRating;
                var newRating = tile.MovementCost - n.MovementCost;
            }
            

            
        }
        return null;
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

    /*public List<OverlayTile> GetTilesInRange(Unit unit, List<OverlayTile> tiles, int range)
    {

        var tilesInRange = new List<OverlayTile>();
        var tilesMarked = new List<OverlayTile>();

        tilesInRange.Add(unit.Tile);

        int count = 0;

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            //Get each tile's distinct neighbor
            tilesInRange.ForEach(t => surroundingTiles.AddRange(t.GetNeighborTiles(new List<OverlayTile>()).Where(n => !n.IsBlocked).Distinct().ToList()));

            foreach(var tile in surroundingTiles.ToList())
            {
                if (!tilesMarked.Contains(tile))
                {
                    tilesMarked.Add(tile);
                    tile.MovementRating = unit.MovementPoints - count - tile.MovementCost;
                    if (tile.MovementRating < 1)
                    {
                        surroundingTiles.Remove(tile);
                    }
                    
                }              
            }
         
            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRange;
    }
    */
}

