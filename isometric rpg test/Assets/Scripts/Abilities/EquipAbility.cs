using System.Collections;
using UnityEngine;

public class EquipAbility : Ability
{
    public Weapon WeaponToEquip { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDisplayable = false;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            UnitReference.EquipWeapon(WeaponToEquip);
        }
        yield return 0;
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid)));
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        base.OnAbilityDeselected(tileGrid);
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.AvailableWeapons.Contains(WeaponToEquip);
    }
}