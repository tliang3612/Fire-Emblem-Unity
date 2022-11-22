using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateMoving : UnitState
{
    private Vector2Int _direction;
    public UnitStateMoving(Unit unit, Vector2Int direction ) : base(unit) { _direction = direction; }

    public override void Apply()
    {
        _unit.SetMove(_direction, true);
    }

    public override void TransitionState(UnitState state)
    {
        state.Apply();
        _unit.UnitState = state;
    }

    public override void OnStateExit()
    {
        _unit.SetMove(Vector2Int.zero, false);
    }

}
