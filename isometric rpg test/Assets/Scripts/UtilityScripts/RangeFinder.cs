using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public class RangeFinder
{
    List<OverlayTile> visitedTiles = new List<OverlayTile>();

    //FloodFill
    public List<OverlayTile> GetTilesInRange(Unit unit, List<OverlayTile> tiles, int range)
    {
        visitedTiles = new List<OverlayTile>();
        unit.Tile.MovementCost = -1;
        unit.Tile.MovementRating = unit.MovementPoints;

        visitedTiles.Add(unit.Tile);

        Flood(unit.Tile);

        return visitedTiles;
    }

    public void Flood(OverlayTile tile)
    {
        if (tile.MovementRating < 0)
            return;
       
        foreach (var n in tile.GetNeighborTiles(new List<OverlayTile>())) 
        {
            //if the neighbor isn't blocked
            if (!n.IsBlocked)
            {
                //if tile has not been visited already
                if (!visitedTiles.Contains(n))
                {
                    visitedTiles.Add(n);
                    n.MovementRating = tile.MovementRating - n.MovementCost;
                }
                else
                {
                    var tempRating = tile.MovementRating - n.MovementCost;
                    n.MovementRating = Math.Max(n.MovementRating, tempRating);
                }

                Flood(n);
            }         
        }
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

