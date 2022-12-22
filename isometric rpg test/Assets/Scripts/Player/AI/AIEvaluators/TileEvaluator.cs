using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEvaluator : MonoBehaviour
{
    public abstract void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid);
    public abstract float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid);
    
}
