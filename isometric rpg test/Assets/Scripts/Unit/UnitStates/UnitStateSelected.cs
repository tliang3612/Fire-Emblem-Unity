
public class UnitStateSelected : UnitState
{
    public UnitStateSelected(Unit unit) : base(unit) { }

    public override void Apply()
    {

    }

    public override void TransitionState(UnitState state)
    {
        state.Apply();
        _unit.UnitState = state;
    }

}
