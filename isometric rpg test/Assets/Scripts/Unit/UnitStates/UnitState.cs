//class that holds the states of the unit

public abstract class UnitState
{
    protected Unit _unit;

    public UnitState(Unit unit)
    {
        _unit = unit;
    }

    public abstract void Apply();

    //Transitions the _unit.UnitState into the desired state
    public abstract void TransitionState(UnitState state);
}


