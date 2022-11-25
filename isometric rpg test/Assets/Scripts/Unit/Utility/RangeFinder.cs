using System.Collections.Generic;
using System.Linq;


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
        Queue<OverlayTile> queue = new Queue<OverlayTile>();

        var start = _unit.Tile;
        var tilesInRange = GetTilesInRange(tileGrid, _unit.MovementPoints);

        queue.Enqueue(start);
        movementRating.Add(start, _unit.MovementPoints);

        //if there is any tile that has been unvisited
        while (queue.Count != 0)
        {
            var currentTile = queue.Dequeue();

            foreach (var neighbor in currentTile.GetNeighborTiles(tileGrid))
            {
                int newRating = movementRating[currentTile] - neighbor.GetMovementCost(_unit.UnitType);

                //if tile is able to be moved to
                if (_unit.IsTileMoveableAcross(neighbor) && tilesInRange.Contains(neighbor))
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
        //create tilesInRange, which is a list of tiles in range of the starting tile
        var tilesInRange = new HashSet<OverlayTile>();
        int count = 0;

        tilesInRange.UnionWith(availableDestinations);

        while (count < range)
        {
            var surroundingTiles = new HashSet<OverlayTile>();

            foreach (var tile in tilesInRange)
            {
                surroundingTiles.UnionWith(tile.GetNeighborTiles(tileGrid));
            }

            tilesInRange.UnionWith(surroundingTiles);
            count++;
        }

        return tilesInRange.Except(availableDestinations).ToHashSet();
    }


    public HashSet<OverlayTile> GetTilesInRange(TileGrid tileGrid, int range)
    {
        var tilesInRange = new HashSet<OverlayTile>();

        tilesInRange.Add(_unit.Tile);

        int count = 0;

        while (count < range)
        {
            var surroundingTiles = new HashSet<OverlayTile>();

            //Get each tile's distinct neighbor
            foreach (var tile in tilesInRange)
            {
                surroundingTiles.UnionWith(tile.GetNeighborTiles(tileGrid));
            }

            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.UnionWith(surroundingTiles);
            count++;
        }
        tilesInRange.Remove(_unit.Tile);

        return tilesInRange;
    }


}


