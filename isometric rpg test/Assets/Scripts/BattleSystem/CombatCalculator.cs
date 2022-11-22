using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class CombatCalculator
{
    private const int _critCoefficient = 2;

    protected CombatStats playerStats;
    protected CombatStats enemyStats;
    protected int range;
    protected int playerHealth;
    protected int enemyHealth;

    public CombatCalculator(CombatStats attackerStats, CombatStats defenderStats, int range)
    {
        playerStats = attackerStats;
        enemyStats = defenderStats;

        //Save a copy of the health to pre run attack events
        playerHealth = attackerStats.HealthStat;
        enemyHealth = defenderStats.HealthStat;
        this.range = range;
    }   
    

    public virtual Queue<BattleAction> Calculate()
    {
        Queue<BattleAction> ret = new Queue<BattleAction>(); 
        List<bool> attackOrder = new List<bool>();

        if (CanAttack(playerStats, enemyStats))
            attackOrder.Add(true);
        
        if (CanAttack(enemyStats, playerStats))
            attackOrder.Add(false);

        
        if ((playerStats.CanDoubleAttack))
        {
            attackOrder.Add(true);
        }

        if (enemyStats.CanDoubleAttack)
        {
            attackOrder.Add(false);
        }

        foreach (bool isPlayerAttack in attackOrder)
        {
            BattleAction currentAction;

            if (isPlayerAttack && CanAttack(playerStats,enemyStats))
            {
                currentAction = RunAction(true, playerStats);
                ret.Enqueue(currentAction);
            }
            else if (!isPlayerAttack && CanAttack(enemyStats, playerStats))
            {
                currentAction = RunAction(false, enemyStats);
                ret.Enqueue(currentAction);
            }           
        }
        return ret;
       
    }

    public virtual BattleAction RunAction(bool isPlayer, CombatStats combatStats)
    {
        int damage;
        
        //modifiers
        int crit = 1;
        int dodge = 1;

        bool isDead = false;
        
        if(UnityEngine.Random.value < (combatStats.CritStat * 0.01))
        {
            crit = _critCoefficient;
        }
        else if (UnityEngine.Random.value > (combatStats.HitStat * 0.01))
        {
            dodge = 0;
        }

        damage = combatStats.DamageStat * crit * dodge;

        if (isPlayer)
            enemyHealth -= damage;
        else
            playerHealth -= damage;

        if (playerHealth <= 0 || enemyHealth <= 0)
            isDead = true;
        
        return new BattleAction(isPlayer, dodge == 1, crit == _critCoefficient, isDead, damage);

    }

    

    public virtual bool CanAttack(CombatStats attackerStats, CombatStats defenderStats)
    {
        if (attackerStats.HealthStat <= 0 || defenderStats.HealthStat <= 0) return false;
        if (attackerStats.RangeStat < range) return false;
        return true;
    }

}

public class BattleAction
{
    public bool IsPlayerAttacking { get; private set; }
    public bool IsHit { get; private set; }
    public bool IsCrit { get; private set; }
    public bool IsDead { get; private set; }
    public int Damage { get; private set; }

    public BattleAction(bool isPlayerAttacking, bool isHit, bool isCrit, bool isDead, int damage)
    {
        IsPlayerAttacking = isPlayerAttacking;
        IsHit = isHit;
        IsCrit = isCrit;
        IsDead = isDead;
        Damage = damage;
    }
}
