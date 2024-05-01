using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitToAttackTileEvaluator : TileEvaluator
{
    private float _bestScore = 0f;
    Dictionary<Unit, List<Tuple<OverlayTile, float>>> _scores;
    private List<Unit> _attackableEnemies;
    private HashSet<OverlayTile> _attackableTiles;

    /// <summary>
    /// This method initializes the damageAndAccuracy dictionary, best damageAndAccuracy score, and attackableEnemiesList
    /// <param name="availableDestinations"></param>
    /// <param name="evaluatingUnit"></param>
    /// <param name="tileGrid"></param>
    /// <param name="availableDestinations"></param><param name="evaluatingUnit"></param><param name="tileGrid"></param><param name="encounterScores"></param>
    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, List<Tuple<OverlayTile, float>>> encounterScores)
    {
        _attackableEnemies = new List<Unit>();
        if (encounterScores.Count == 0)
        {
            _bestScore = 1f;
            _scores = encounterScores;
            return;
        }

        // get the best score out of each tile
        _bestScore = encounterScores.Values.Select(list => list.Select(pair => { 
            return pair.Item2; 
        }).Max()).Max();
        _scores = encounterScores;

        _attackableTiles = evaluatingUnit.GetTilesInAttackRange(tileGrid);

        foreach (var tile in _attackableTiles)
        {
            var enemy = tile.CurrentUnit;
            //we check if the unit exists and can be attacked
            if (enemy && evaluatingUnit.PlayerNumber != enemy.PlayerNumber)
                _attackableEnemies.Add(enemy);
            else
                continue;
        }
    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        if (_attackableEnemies.Count() <= 0)
            return 0f;

        var scores = _attackableEnemies.Select(e =>
        {
            if (!_scores.ContainsKey(e))
                return 0f;

            var score = 0f;

            // Get the associated score for the enemy that corresponds to the tile given
            if (_scores[e].Where(pair => pair.Item1.Equals(tileToEvaluate)).Any())
            {
                score = _scores[e].Where(pair => pair.Item1.Equals(tileToEvaluate)).FirstOrDefault().Item2;
            }
            return score/_bestScore; 
        });
        return scores.Min() * 2.5f; // we return min because we want to evaluate the worst case scenario, rather than the best case
                                    // * 2.5f is because we want the weight of this evalutor to be worth more than the others
    }
}
