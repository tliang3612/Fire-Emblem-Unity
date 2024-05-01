using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyProximityTileEvaluator : TileEvaluator
{
    private Unit _closestEnemy;
    private float _closestEnemyDistance;
    //

    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, List<Tuple<OverlayTile, float>>> encounterScores)
    {
        var enemies = tileGrid.GetEnemyUnits(evaluatingUnit.Player).OrderBy(e => tileGrid.GetManhattenDistance(e.Tile, evaluatingUnit.Tile)).ToList();
        _closestEnemy = enemies.FirstOrDefault();
        _closestEnemyDistance = tileGrid.GetManhattenDistance(_closestEnemy.Tile, evaluatingUnit.Tile);
    }

    
    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        float distance = tileGrid.GetManhattenDistance(tileToEvaluate, _closestEnemy.Tile);

        //the closer we are to the closest enemy, the higher the score. If the distance is greater than _closestEnemyDistance, score is negative -1
        //this is to prevent ai from running away from enemies if possible
        return distance <= _closestEnemyDistance ? 2 - (distance/_closestEnemyDistance) : 0f;
    } 

    
}
