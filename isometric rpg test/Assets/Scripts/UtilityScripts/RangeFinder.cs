using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public class RangeFinder
{
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

    public List<OverlayTile> GetTilesInMoveRange(Unit unit, TileGrid tileGrid, List<OverlayTile> tilesInRange)
    {

        Dictionary<OverlayTile, int> movementRating = new Dictionary<OverlayTile, int>();
        Queue<OverlayTile> queue = new Queue<OverlayTile>();

        var start = unit.Tile;

        queue.Enqueue(start);
        movementRating.Add(start, unit.MovementPoints);


        //if there is any tile that has been unvisited
        while (queue.Count != 0)
        {
            var currentTile = queue.Dequeue();

            foreach (var neighbor in currentTile.GetNeighborTiles(tileGrid))
            {
                int newRating = movementRating[currentTile] - neighbor.MovementCost;

                //if tile is able to be moved to
                if (!neighbor.IsBlocked && tilesInRange.Contains(neighbor))
                {
                    //if tile has not been visited or the newRating of the tile is greater than its previous rating
                    if (!movementRating.ContainsKey(neighbor) || newRating > movementRating[neighbor])
                    {
                        movementRating[neighbor] = newRating;
                        queue.Enqueue(neighbor);
                    }
                   
                }
            }

        }

        List<OverlayTile> availableTiles = new List<OverlayTile>();

        //foreach tile in movementRating that has a rating of above 0, it is movable to
        foreach (var tile in movementRating.Keys)
        {
            if(movementRating[tile] > 0)
            {
                availableTiles.Add(tile);
            }
        }

        return availableTiles;
    }
}


