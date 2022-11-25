using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MoveAbility), typeof(DisplayActionsAbility))]
public class WaitAbility : Ability
{

    protected override void Awake()
    {
        base.Awake();
        Name = "Wait";
        IsDisplayable = true;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            UnitReference.ConfirmMove();
            UnitReference.SetState(new UnitStateFinished(UnitReference));            
        }
        yield return 0;
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid)));
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return true;
    }


}


