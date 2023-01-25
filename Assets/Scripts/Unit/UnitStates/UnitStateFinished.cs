using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitStateFinished : UnitState
{
    //Constructor
    public UnitStateFinished(Unit unit) : base(unit)
    {
    }

    public override void Apply()
    {
        _unit.MarkAsFinished();
        _unit.SetFinished();
    }

    public override void TransitionState(UnitState state)
    {
        //finished state must transition to normal state
        if(state is UnitStateNormal)
        {
            OnStateExit();
            state.Apply();
            _unit.UnitState = state;
        }
    }

    public override void OnStateExit()
    {
        
    }
}

