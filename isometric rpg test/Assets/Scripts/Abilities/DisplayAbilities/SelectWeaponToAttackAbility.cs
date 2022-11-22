using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DisplayAttackStatsAbility), typeof(DisplayActionsAbility))]
public class SelectWeaponToAttackAbility : SelectItemToUseAbility
{

    private List<Unit> _enemiesInRange;
    private int _range;

    protected override void Awake()
    {
        base.Awake();
        Name = "Attack";
        IsDisplayable = true;
    }

    public override void Display(TileGrid tileGrid)
    {
        foreach (Weapon w in UnitReference.AvailableWeapons) 
        {
            if(w.Range >= _range)
                OnButtonCreated(ActWrapper(w, tileGrid), w.Name, w);                   
        }
    }

    public IEnumerator ActWrapper(Weapon w, TileGrid tileGrid)
    {
        return Execute(tileGrid,
                _ => UnitReference.EquipItem(w),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, GetComponent<DisplayAttackStatsAbility>())); ;
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);
        if (_enemiesInRange.Count > 0)
            _range = _enemiesInRange.Min(e => tileGrid.GetManhattenDistance(UnitReference.Tile, e.Tile));
    }

    public override void OnRightClick(TileGrid tileGrid)
    {
        StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<DisplayActionsAbility>()));
    }

    //Can Select Weapon to Attack if the Unit has a weapon
    public override bool CanPerform(TileGrid tileGrid)
    {
        if (UnitReference.AvailableWeapons.Count <= 0)
        {
            return false;
        }

        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        _enemiesInRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, false)).ToList();

        //the distance of the closest enemy

        return _enemiesInRange.Count > 0 && UnitReference.ActionPoints > 0;
    }
}

