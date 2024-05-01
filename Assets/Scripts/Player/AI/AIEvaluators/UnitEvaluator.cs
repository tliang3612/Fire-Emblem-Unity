using System.Collections.Generic;
using UnityEngine;

public abstract class UnitEvaluator : MonoBehaviour
{
    public abstract void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, float> encounterScores);
    public abstract float Evaluate(Unit unitToEvaluate, Unit evaluatingUnit, TileGrid tileGrid);   
}

