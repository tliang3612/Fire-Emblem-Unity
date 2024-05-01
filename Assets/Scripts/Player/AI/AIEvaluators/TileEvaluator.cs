using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEvaluator : MonoBehaviour
{
    public abstract void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, List<Tuple<OverlayTile, float>>> encounterScores);
    public abstract float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid);
    
}
