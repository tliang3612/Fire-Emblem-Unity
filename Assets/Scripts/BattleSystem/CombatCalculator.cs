using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatCalculator
{
    private const int _critCoefficient = 2;

    private CombatStats _playerStats;
    private CombatStats _enemyStats;
    private int _tempPlayerHealth;
    private int _tempEnemyHealth;
    private int _range;

    public CombatCalculator(CombatStats attackerStats, CombatStats defenderStats, int range)
    {
        _playerStats = attackerStats;
        _enemyStats = defenderStats;

        //Save a copy of the health to pre run attack events
        _tempPlayerHealth = attackerStats.HealthStat;
        _tempEnemyHealth = defenderStats.HealthStat;
        _range = range;
    }   
    

    public virtual Queue<BattleAction> Calculate()
    {
        Queue<BattleAction> ret = new Queue<BattleAction>(); 
        List<bool> attackOrder = GetAttackOrder(); 

        foreach (bool isPlayerAttack in attackOrder)
        {
            BattleAction currentAction;

            if (isPlayerAttack && CanAttack(_playerStats,_enemyStats))
            {
                currentAction = RunAction(true, _playerStats);
                ret.Enqueue(currentAction);
            }
            else if (!isPlayerAttack && CanAttack(_enemyStats, _playerStats))
            {
                currentAction = RunAction(false, _enemyStats);
                ret.Enqueue(currentAction);
            }           
        }
        return ret;
       
    }

    public virtual List<bool> GetAttackOrder()
    {
        List<bool> attackOrder = new List<bool>();

        if (CanAttack(_playerStats, _enemyStats))
            attackOrder.Add(true);

        if (CanAttack(_enemyStats, _playerStats))
            attackOrder.Add(false);


        if ((_playerStats.CanDoubleAttack))
        {
            attackOrder.Add(true);
        }

        if (_enemyStats.CanDoubleAttack)
        {
            attackOrder.Add(false);
        }

        return attackOrder;
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
            _tempEnemyHealth -= damage;
        else
            _tempPlayerHealth -= damage;

        if (_tempPlayerHealth <= 0 || _tempEnemyHealth <= 0)
            isDead = true;
        
        return new BattleAction(isPlayer, dodge == 1, crit == _critCoefficient, isDead, damage);

    }



    public virtual bool CanAttack(CombatStats attackerStats, CombatStats defenderStats)
    {
        if (attackerStats.HealthStat <= 0 || defenderStats.HealthStat <= 0) return false;
        if (attackerStats.RangeStat < _range) return false;
        return true;
    }

}

public struct BattleAction
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
