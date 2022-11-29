using System.Collections.Generic;
using Unity.VisualScripting;

public class AStarPathfinder
{

    private Unit _unit;

    public AStarPathfinder(Unit unit)
    {
        _unit = unit;
    }

    //Find all from the unit's starting tile
    public Dictionary<OverlayTile, List<OverlayTile>> FindAllPaths(HashSet<OverlayTile> searchableTiles, TileGrid tileGrid)
    {
        //Finished Dictionary
        Dictionary<OverlayTile, List<OverlayTile>> paths = new Dictionary<OverlayTile, List<OverlayTile>>();

        //for each searchable tile, find the best path for the destination
       foreach(OverlayTile tile in searchableTiles)
       {
            paths.Add(tile, FindPath(tile, searchableTiles, tileGrid));
       }

        return paths;
    }

    public Dictionary<OverlayTile, List<OverlayTile>> FindAllBestPaths(HashSet<OverlayTile> searchableTiles, TileGrid tileGrid)
    {
        //Finished Dictionary<EndTile, PathToEndTile>
        Dictionary<OverlayTile, List<OverlayTile>> paths = new Dictionary<OverlayTile, List<OverlayTile>>();

        //Finished List
        List<OverlayTile> path;

        var start = _unit.Tile;

        PriorityQueue<OverlayTile> frontier;
        Dictionary<OverlayTile, OverlayTile> previousTile = new Dictionary<OverlayTile, OverlayTile>();
        Dictionary<OverlayTile, int> costSoFar;


        foreach(OverlayTile end in searchableTiles)
        {
            frontier = new PriorityQueue<OverlayTile>();
            costSoFar = new Dictionary<OverlayTile, int>();

            path = new List<OverlayTile>();


            frontier.Enqueue(start, 0);
            costSoFar.Add(start, 0);
            previousTile.Add(start, default(OverlayTile));

            if (start == end)
            {
                path.Add(start);
                paths.Add(end, path);
                continue;
            }
            
            //if the end tile has 
            if(previousTile.ContainsKey(end))
            {

            }

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
            {
                paths.Add(end, path);
                continue;
            }
                
            path.Add(end);
            var temp = end;

            while (!previousTile[temp].Equals(start))
            {
                path.Add(previousTile[temp]);
                temp = previousTile[temp];

            }

            path.Add(start);
            path.Reverse();
        }

        return paths;
    }


    //Djikstra
    public List<OverlayTile> FindPath(OverlayTile end, HashSet<OverlayTile> searchableTiles, TileGrid tileGrid)
    {
        //Finished List
        List<OverlayTile> path = new List<OverlayTile>();

        var start = _unit.Tile;

        if (start == end)
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
