using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinder
{
    
    //Takes the starting node and the end node
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles, TileGrid tileGrid)
    {
        //list of tiles to be checked in the next iteration
        List<OverlayTile> openList = new List<OverlayTile>();
        //list of tiles that are already checked
        List<OverlayTile> closedList = new List<OverlayTile>();

        openList.Add(start);

        while(openList.Count > 0)
        {
            //gets the overlayTile with the lowest F value to find optimal tile to move to next
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if(currentOverlayTile == end)
            {
                return GetFinishedList(start, end);
            }


            foreach (var neighbor in tileGrid.GetNeighborTiles(currentOverlayTile, searchableTiles))
            {
                                          //if checked tile list contains this neighbor tile
                if (neighbor.IsBlocked || closedList.Contains(neighbor))
                {
                    continue;
                }

                //calculate mahatten distance (non-diagonal distance)
                neighbor.G = GetManhattenDistance(start, neighbor);
                neighbor.H = GetManhattenDistance(end, neighbor);

                neighbor.previous = currentOverlayTile;

                //adds neighbor tiles to openList to check in the next iteration
                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }
        }

        return new List<OverlayTile>();
    }

    //return a list of tiles that the character will traverse in order
    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();

        OverlayTile currentTile = end;

        int movesTaken = 0;

        while(currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
            movesTaken++;
        }
        finishedList.Reverse();
        return finishedList;
    }
    
    //Manhattan (A,B) = |x1-x2| + |y1-y2|
    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbor)
    {
        return Mathf.Abs(start.gridLocation.x - neighbor.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbor.gridLocation.y);
    }

    
}
