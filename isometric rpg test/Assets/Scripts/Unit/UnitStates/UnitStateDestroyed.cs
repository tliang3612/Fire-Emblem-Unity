


public class UnitStateDestroyed : UnitState
{
    //Constructor
    public UnitStateDestroyed(Unit unit) : base(unit)
    {
    }

    public override void Apply()
    {
        _unit.MarkAsDestroyed();
    }

    public override void TransitionState(UnitState state)
    {
        //finished state must transition to normal state
        if (state is UnitStateNormal)
        {
            state.Apply();
            _unit.UnitState = state;
        }
    }
}

