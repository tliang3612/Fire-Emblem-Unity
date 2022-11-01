using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateNormal : UnitState
{
        
    public UnitStateNormal(Unit unit) : base(unit){ }

    public override void Apply()
    {
        _unit.SetAnimationToIdle();
    }

    public override void TransitionState(UnitState state)
    {
        state.Apply();
        _unit.UnitState = state;
    }

}
