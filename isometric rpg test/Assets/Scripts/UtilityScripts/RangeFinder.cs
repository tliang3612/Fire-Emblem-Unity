using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class RangeFinder
{

    //FloodFill
    public List<OverlayTile> GetTilesInRange(Unit unit, List<OverlayTile> tiles, int range)
    {
        //create tilesInRange, which is a list of tiles in range of the starting tile
        var tilesInRange = new List<OverlayTile>();

        int count = 0;

        tilesInRange.AddRange(tiles);

        while (count < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            tilesInRange.ForEach(t => surroundingTiles.AddRange(t.GetNeighborTiles(new List<OverlayTile>()).Where(n => !n.IsBlocked).ToList()));

            foreach(var tile in surroundingTiles)
            {
                tile.MovementRating -= tile.MovementCost;
                if(tile.MovementRating < 0)
                {
                    surroundingTiles.Remove(tile);
                }
            }
         
            //adds the surrounding distinct tiles to tilesInRange
            tilesInRange.AddRange(surroundingTiles.Distinct().ToList());
            count++;
        }

        return tilesInRange;
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

}

   