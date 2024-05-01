using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTileEvaluator : TileEvaluator
{

    float _bestScore;

    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, List<Tuple<OverlayTile, float>>> encounterScores)
    {
        _bestScore = 0f;
        foreach (var tile in availableDestinations)
        {
            var tileScore = 0f;
            tileScore += tile.DefenseBoost + tile.AvoidBoost / 10f;

            if(tileScore > _bestScore)
            {
                _bestScore = tileScore;
            }
        }
    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        return (tileToEvaluate.DefenseBoost + tileToEvaluate.AvoidBoost / 10f) / _bestScore * .5f;
    }

    
}
