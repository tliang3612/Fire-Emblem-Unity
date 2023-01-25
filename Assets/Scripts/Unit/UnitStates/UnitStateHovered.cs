using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateHovered : UnitState
{
    public UnitStateHovered(Unit unit) : base(unit) { }

    public override void Apply()
    {
        _unit.SetAnimationToSelected(true);
    }

    public override void TransitionState(UnitState state)
    {
        state.Apply();
        _unit.UnitState = state;
    }

    public override void OnStateExit()
    {
        _unit.SetAnimationToSelected(false);
        _unit.SetAnimationToIdle();
    }

}
