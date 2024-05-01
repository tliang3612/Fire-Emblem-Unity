using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitToAttackUnitEvaluator : UnitEvaluator
{
    private float _bestScore = 0f;
   
    private Dictionary<Unit, float> _scores;

    public override void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, float> encounterScores)
    {
        if (encounterScores.Count == 0)
        {
            _bestScore = 1f;
            _scores = encounterScores;
            return;
        }


        _bestScore = encounterScores.Values.Max();
        _scores = encounterScores;

    }

    public override float Evaluate(Unit unitToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        if (!_scores.ContainsKey(unitToEvaluate))
            return 0f;

        return _scores[unitToEvaluate] / _bestScore;
    }
}