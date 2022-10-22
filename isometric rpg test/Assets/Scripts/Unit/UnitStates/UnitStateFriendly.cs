
public class UnitStateFriendly : UnitState
{
    public UnitStateFriendly(Unit unit) : base(unit) { }

    public override void Apply()
    {
        _unit.MarkAsFriendly();
    }

    public override void TransitionState(UnitState state)
    {
        state.Apply();
        _unit.UnitState = state;
    }
}

