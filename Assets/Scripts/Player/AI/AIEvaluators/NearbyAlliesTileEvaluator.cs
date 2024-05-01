using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NearbyAlliesTileEvaluator : TileEvaluator
{
    private float _bestScore = 0f;
    private Dictionary<OverlayTile, float> _scores;

    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, List<Tuple<OverlayTile, float>>> encounterScores)
    {
        _scores = new Dictionary<OverlayTile, float>();

        foreach (var tile in availableDestinations)
        {
            var nearbyAlliesScore = 0f;
            foreach (var ally in tileGrid.GetCurrentPlayerUnits())
            {
                // see if the tile is within 3 ranges of an ally. 3 instead of 1 so that ai doesnt clump
                if (ally.GetTilesInRange(tileGrid, 3).Contains(tile))
                {
                    float distanceFromAlly = tileGrid.GetManhattenDistance(tile, ally.Tile);
                    nearbyAlliesScore += distanceFromAlly / 2f; // The closer the distance, the worse the score. This is so that the AI controls more range, but stay within 3 ranges
                }
            }
            _scores[tile] = nearbyAlliesScore;

            // maximum best score should be 2.5f, an abritrary number. Anything more would make units clump together
            if (nearbyAlliesScore > 2.5f)
            {
                _bestScore = nearbyAlliesScore;
                break;
            }
            else if(nearbyAlliesScore > _bestScore)
            {
                _bestScore = nearbyAlliesScore;
            }

        }
    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        if (!_scores.ContainsKey(tileToEvaluate))
            return 0f;

        return _scores[tileToEvaluate]/_bestScore * .5f; // I dont want this evaluator to be too heavily weighted
    }    
}
