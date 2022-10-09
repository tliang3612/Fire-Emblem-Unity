using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public class RangeFinder
{
    public List<OverlayTile> visitedTiles = new List<OverlayTile>();

    //FloodFill
    public List<OverlayTile> GetTilesInMoveRange(Unit unit, TileGrid tileGrid)
    {
        return GetTilesInRange(unit, tileGrid, 2);
        /*visitedTiles = new List<OverlayTile>();
        unit.Tile.MovementCost = -1;
        unit.Tile.MovementRating = unit.MovementPoints;

        visitedTiles.Add(unit.Tile);

        Flood(unit.Tile, tileGrid);

        return visitedTiles;*/
    }

    public void Flood(OverlayTile tile, TileGrid tileGrid)
    {
        if (tile.MovementRating < 1)
            return;
       
        foreach (var n in tile.GetNeighborTiles(tileGrid)) 
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
                    n.MovementRating = Mathf.Max(n.MovementRating, tempRating);
                }         
            }
            Flood(n, tileGrid);
        }
    }

    public List<OverlayTile> GetTilesInAttackRange(List<OverlayTile> availableDestinations, TileGrid tileGrid, int range)
    {
        //create tilesInRange, which is a list of tiles in range of the starting tile
        var tilesInRange = new List<OverlayTile>();
        int count = 0;

        tilesInRange.AddRange(availableDestinations);

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            tilesInRange.ForEach(t => surroundingTiles.AddRange(t.GetNeighborTiles(tileGrid)));

            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRange.Except(availableDestinations).ToList();
    }

    public List<OverlayTile> GetTilesInRange(Unit unit, TileGrid tileGrid, int range)
    {
        var tilesInRange = new List<OverlayTile>();

        tilesInRange.Add(unit.Tile);

        int count = 0;

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            //Get each tile's distinct neighbor
            tilesInRange.ForEach(t => surroundingTiles.AddRange(t.GetNeighborTiles(tileGrid)));
         
            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRange;
    }
}

