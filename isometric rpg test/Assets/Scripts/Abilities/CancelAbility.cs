using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CancelAbility : Ability
{

    protected override void Awake()
    {
        base.Awake();
        Name = "Cancel";
        IsDisplayable = true;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            UnitReference.ResetMove();
        }
        yield return 0;
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponentInChildren<MoveAbility>())));
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        UnitReference.SetAnimationToIdle();
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return true;
    }
}


