using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class CombatCalculator
{
    protected Unit playerUnit, enemyUnit;
    protected int range;

    public CombatCalculator(Unit attacker, Unit defender, int range)
    {
        playerUnit = attacker;
        enemyUnit = defender;
        this.range = range;
    }   
    
    public Queue<BattleAction> Calculate()
    {
        Queue<BattleAction> ret = new Queue<BattleAction>(); 
        List<bool> attackOrder = new List<bool>();

        if (CanAttack(playerUnit, enemyUnit, range))
            attackOrder.Add(true);
        
        if (CanAttack(enemyUnit, playerUnit, range))
            attackOrder.Add(false);

        foreach(bool isPlayerAttack in attackOrder)
        {
            BattleAction currentAction;

            if (isPlayerAttack && CanAttack(playerUnit, enemyUnit, range))
            {
                currentAction = RunAction(true);
                ret.Enqueue(currentAction);
            }
            else if (!isPlayerAttack && CanAttack(enemyUnit, playerUnit, range))
            {
                currentAction = RunAction(false);
                ret.Enqueue(currentAction);
            }           
        }
        Debug.Log(ret.Count);
        return ret;

        
    }

    public BattleAction RunAction(bool isPlayer)
    {
        Unit attacker = isPlayer ? playerUnit : enemyUnit;
        Unit defender = isPlayer ? enemyUnit : playerUnit;

        int damage;
        
        //modifiers
        int crit = 1;
        int dodge = 1;


        if(UnityEngine.Random.value < (attacker.GetCritChance() * 0.01))
        {
            crit = 2;
        }
        else if (UnityEngine.Random.value > (GetBattleAccuracy(attacker, defender) * 0.01))
        {
            dodge = 0;
        }

        damage = attacker.GetTotalDamage(defender) * crit * dodge;

        defender.ReceiveDamage(damage);

        return new BattleAction(isPlayer, dodge == 1, crit == 2);


    }

    public int GetBattleAccuracy(Unit attacker, Unit defender)
    {
        //Weapon Effectiveness mutiplier for hit chance = 15;
        int weaponEffectiveness = attacker.GetEffectiveness(defender) * 15;

        return Mathf.Clamp((attacker.GetHitChance() + weaponEffectiveness) - defender.GetDodgeChance(), 0, 100);
    }

    public int GetTotalDamage(Unit attacker, Unit defender)
    {
        //Weapon Effectiveness mutiplier = 2;
        int weaponEffectiveness = attacker.GetEffectiveness(defender) * 2;

        return (attacker.GetAttack() + weaponEffectiveness) - defender.GetDefense(); 
    }

    public bool CanAttack(Unit attacker, Unit defender, int range)
    {
        if (attacker.HitPoints <= 0 || defender.HitPoints <= 0) return false;
        if (attacker.AttackRange < range) return false;
        return true;
    }

}

public class BattleAction
{
    public bool IsPlayerAttacking;
    public bool IsHit;
    public bool IsCrit;

    public BattleAction(bool isPlayerAttacking, bool isHit, bool isCrit)
    {
        IsPlayerAttacking = isPlayerAttacking;
        IsHit = isHit;
        IsCrit = isCrit;
    }
}

