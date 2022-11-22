using System.Collections;
using UnityEngine;

//This ability is used to Set the unit back to a unit deselected state
[RequireComponent(typeof(MoveAbility))]
public class ResetAbility : Ability
{

    protected override void Awake()
    {
        base.Awake();
        IsDisplayable = false;
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);

        UnitReference.SetState(new UnitStateNormal(UnitReference));
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return true;
    }
}