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
            UnitReference.InSelectionMenu = false;
        }
        yield return 0;
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponent<MoveAbility>())));
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.InSelectionMenu && UnitReference.storedMovementDetails != null;
    }


}


