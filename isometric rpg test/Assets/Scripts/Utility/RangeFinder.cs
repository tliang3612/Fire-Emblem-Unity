using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    private Unit _unit;

    public RangeFinder(Unit unit)
    {
        _unit = unit;
    }

    //Modified Djikstra
    public HashSet<OverlayTile> GetTilesInMoveRange(TileGrid tileGrid)
    {
        Dictionary<OverlayTile, int> movementRating = new Dictionary<OverlayTile, int>();
        Queue<OverlayTile> tilesToExplore = new Queue<OverlayTile>();

        var start = _unit.Tile;
        //var tilesInRange = GetTilesInRange(tileGrid, _unit.MovementPoints);

        tilesToExplore.Enqueue(start);
        movementRating.Add(start, _unit.MovementPoints);

        //if there is any tile that has been unvisited
        while (tilesToExplore.Count != 0)
        {
            var currentTile = tilesToExplore.Dequeue();

            foreach (var neighbor in currentTile.GetNeighborTiles(tileGrid))
            {
                int newRating = movementRating[currentTile] - neighbor.GetMovementCost(_unit.UnitType);

                //if tile is able to be moved to
                if (_unit.IsTileMoveableAcross(neighbor))
                {
                    //if tile has not been visited or the newRating of the tile is greater than its previous rating
                    if (!movementRating.ContainsKey(neighbor) || newRating > movementRating[neighbor])
                    {
                        movementRating[neighbor] = newRating;
                        tilesToExplore.Enqueue(neighbor);
                    }

                }
            }
        }

        HashSet<OverlayTile> availableTiles = new HashSet<OverlayTile>();

        //foreach tile in movementRating that has a rating of above 0, it is movable to
        foreach (var tile in movementRating.Keys)
        {
            if (movementRating[tile] > 0)
            {
                availableTiles.Add(tile);
            }
        }
        return availableTiles;
    }

    public HashSet<OverlayTile> GetTilesInAttackRange(HashSet<OverlayTile> availableDestinations, TileGrid tileGrid, int range)
    {
        HashSet<OverlayTile> endTiles = new HashSet<OverlayTile>();
        HashSet<OverlayTile> exploredTiles = new HashSet<OverlayTile>();

        exploredTiles.UnionWith(availableDestinations);
        foreach (var tile in availableDestinations)
        {
            foreach(var neighbor in tile.GetNeighborTiles(tileGrid))
            {
                if (!availableDestinations.Contains(neighbor))
                {
                    endTiles.Add(tile);
                }
            }           
        }
        
        int count = 0;

        while(count < range)
        {
            HashSet<OverlayTile> newEndTiles = new HashSet<OverlayTile>();
            foreach (var tile in endTiles)
            {
                foreach(var neighbor in tile.GetNeighborTiles(tileGrid))
                {
                    if (!exploredTiles.Contains(neighbor))
                    {
                        exploredTiles.Add(neighbor);
                        newEndTiles.Add(neighbor);
                    }                       
                }           
            }
            endTiles = newEndTiles;
            count++;
        }

        return exploredTiles.ToHashSet();
    }

    public HashSet<OverlayTile> GetTilesInRange(TileGrid tileGrid, int range)
    {
        Dictionary<OverlayTile, int> movementRating = new Dictionary<OverlayTile, int>();
        Queue<OverlayTile> tilesToExplore = new Queue<OverlayTile>();

        var start = _unit.Tile;

        tilesToExplore.Enqueue(start);
        movementRating.Add(start, range);

        //if there is any tile that has been unvisited
        while (tilesToExplore.Count != 0)
        {
            var currentTile = tilesToExplore.Dequeue();

            foreach (var neighbor in currentTile.GetNeighborTiles(tileGrid))
            {
                int newRating = movementRating[currentTile] - 1;

                //if tile has not been visited or the newRating of the tile is greater than its previous rating
                if (!movementRating.ContainsKey(neighbor) || newRating > movementRating[neighbor])
                {
                    movementRating[neighbor] = newRating;
                    tilesToExplore.Enqueue(neighbor);
                }
               
            }
        }

        HashSet<OverlayTile> availableTiles = new HashSet<OverlayTile>();

        //foreach tile in movementRating that has a rating of above 0, it is movable to
        foreach (var tile in movementRating.Keys)
        {
            if (movementRating[tile] >= 0 && tile != _unit.Tile)
            {
                availableTiles.Add(tile);
            }
        }
        return availableTiles;
    }
}


