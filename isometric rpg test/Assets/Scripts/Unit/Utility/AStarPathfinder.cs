using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinder
{

    private Unit _unit;

    public AStarPathfinder(Unit unit)
    {
        _unit = unit;
    }

    //Djikstra
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles, TileGrid tileGrid)
    {     
        //Finished List
        List<OverlayTile> path = new List<OverlayTile>();
        
        if(start == end)
        {
            path.Add(start);
            return path;
        }

        PriorityQueue<OverlayTile> frontier = new PriorityQueue<OverlayTile>();
        Dictionary<OverlayTile, OverlayTile> previousTile = new Dictionary<OverlayTile, OverlayTile>();
        Dictionary<OverlayTile, int> costSoFar = new Dictionary<OverlayTile, int>();

        frontier.Enqueue(start, 0);
        costSoFar.Add(start, 0);
        previousTile.Add(start, default(OverlayTile));

        while (frontier.Count != 0)
        {
            var currentTile = frontier.Dequeue();
            if (currentTile.Equals(end))
                break;

            foreach (var neighbor in currentTile.GetNeighborTiles(tileGrid))
            {
                var newCost = costSoFar[currentTile] + neighbor.GetMovementCost(_unit.UnitType);

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    //if the neighbor tile is moveable across
                    if (_unit.IsTileMoveableAcross(neighbor) && searchableTiles.Contains(neighbor))
                    {
                        costSoFar[neighbor] = newCost;
                        int priority = newCost + tileGrid.GetManhattenDistance(start, neighbor);
                        frontier.Enqueue(neighbor, priority);
                        previousTile[neighbor] = currentTile;

                    }
                }
            }
        }

        //if end tile doesn't have a previous tile, it is unreachable
        if (!previousTile.ContainsKey(end))
            return path;

        path.Add(end);
        var temp = end;


        while (!previousTile[temp].Equals(start))
        {
            path.Add(previousTile[temp]);
            temp = previousTile[temp];
        }

        path.Add(start);

        path.Reverse();
        return path;

    }
}
