using UnityEngine;

public abstract class UnitEvaluator : MonoBehaviour
{
    public abstract void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid);
    public abstract float Evaluate(Unit unitToEvaluate, Unit evaluatingUnit, TileGrid tileGrid);   
}

