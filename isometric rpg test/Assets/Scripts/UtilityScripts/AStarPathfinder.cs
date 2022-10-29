using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinder
{
    //Djikstra
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles, TileGrid tileGrid)
    {
        PriorityQueue<OverlayTile> frontier = new PriorityQueue<OverlayTile>();
        //Finished List
        List<OverlayTile> path = new List<OverlayTile>();
        Dictionary<OverlayTile, OverlayTile> previousTile = new Dictionary<OverlayTile, OverlayTile>();
        Dictionary<OverlayTile, int> costSoFar = new Dictionary<OverlayTile, int>();

        if(start == end)
        {
            path.Add(start);
            return path;
        }

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
                var newCost = costSoFar[currentTile] + neighbor.MovementCost;

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    if (!neighbor.IsBlocked && searchableTiles.Contains(neighbor))
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
