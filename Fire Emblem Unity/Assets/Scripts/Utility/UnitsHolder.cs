using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class UnitsHolder : MonoBehaviour
{
    public void AddComponentsToUnit(Unit unit, List<Player> playersList)
    {

        GameObject holder = new GameObject("ComponentsHolder");
        holder.transform.parent = unit.transform;

        holder.AddComponent<MoveAbility>();
        holder.AddComponent<DisplayActionsAbility>();
        holder.AddComponent<ResetAbility>();

        if (unit.EquippedWeapon)
        {
            holder.AddComponent<AttackAbility>();
            holder.AddComponent<DisplayAttackStatsAbility>();
            holder.AddComponent<SelectWeaponToAttackAbility>();
            holder.AddComponent<SelectWeaponToEquipAbility>();
        }

        if (unit.EquippedStaff)
        {
            holder.AddComponent<HealAbility>();
            holder.AddComponent<DisplayHealStatsAbility>();
            holder.AddComponent<SelectStaffToHealAbility>();
        }

        holder.AddComponent<WaitAbility>();

        foreach (var player in playersList)
        {
            if (unit.PlayerNumber == player.PlayerNumber)
            {
                unit.Player = player;

                if (player is SmartAIPlayer)
                {
                    holder.AddComponent<UnitToAttackUnitEvaluator>();
                    holder.AddComponent<UnitHealthUnitEvaluator>();
                    holder.AddComponent<UnitToAttackTileEvaluator>();
                    holder.AddComponent<DamageTakenTileEvaluator>();
                    holder.AddComponent<NearbyAlliesTileEvaluator>();
                    holder.AddComponent<EnemyProximityTileEvaluator>();
                }
            }
        }

        
    }

    [ExecuteInEditMode]
    public void SnapAllUnits()
    {
        var units = gameObject.GetComponentsInChildren<Unit>();

        //round to the closest 0.5f value, so 3.1f => 3.0f and 3.7f => 4.0f
        foreach(var unit in units)
        {
            var x = unit.transform.position.x - Mathf.RoundToInt(unit.transform.position.x) < 0 ? -1 : 1;
            var y = unit.transform.position.y - Mathf.RoundToInt(unit.transform.position.y) < 0 ? -1 : 1;

            unit.transform.position = new Vector2(Mathf.RoundToInt(unit.transform.position.x) + 0.5f * x, Mathf.RoundToInt(unit.transform.position.y) + 0.5f * y);
        }
    }
}

[CustomEditor(typeof(UnitsHolder))]
[CanEditMultipleObjects]
public class UnitsHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UnitsHolder holder = (UnitsHolder)target;

        if (GUILayout.Button("Snap Units"))
        {
            holder.SnapAllUnits();
        }
    }
}

